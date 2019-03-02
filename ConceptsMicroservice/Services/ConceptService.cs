/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Auth0.AuthenticationApi.Models;
using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class ConceptService : BaseService, IConceptService
    {
        private readonly IConceptRepository _conceptRepository;
        private readonly IConceptMediaRepository _conceptMediaRepository;
        private readonly IStatusRepository _statusRepository;

        public ConceptService(IConceptRepository concept,  IStatusRepository status, IConceptMediaRepository media, IMapper mapper, IUrlHelper urlHelper) : base(mapper, urlHelper)
        {
            _conceptRepository = concept;
            _statusRepository = status;
            _conceptMediaRepository = media;
        }
        
        public Response SearchForConcepts(ConceptSearchQuery query)
        {
            try
            {
                var concepts = _conceptRepository.SearchForConcepts(query); 
                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = concepts.FirstOrDefault().TotalItems;
                    numberOfPages = concepts.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var res = new PagingDTO<ConceptDto>(
                    Mapper.Map<List<ConceptDto>>(concepts),
                    query,
                    UrlHelper.Action("Search", "Concept", query), 
                    numberOfPages,
                    totalItems);
                

                return new Response
                {
                    Data = res
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Response GetConceptById(int id)
        {
            try
            {
                return new Response
                {
                    Data = Mapper.Map<ConceptDto>(_conceptRepository.GetById(id))
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Response GetAllConcepts(BaseListQuery query)
        {
            try
            {
                var concepts = _conceptRepository.GetAll(query);
                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = concepts.FirstOrDefault().TotalItems;
                    numberOfPages = concepts.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var res = new PagingDTO<ConceptDto>(
                    Mapper.Map<List<ConceptDto>>(concepts),
                    query,
                    UrlHelper.Action("GetAll", "Concept", query),
                    numberOfPages,
                    totalItems);

                return new Response
                {
                    Data = res
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Response UpdateConcept(UpdateConceptDto dto)
        {
            var newConceptVersion = Mapper.Map<Concept>(dto);
            var viewModel = new Response();
            var oldConceptVersion = _conceptRepository.GetById(newConceptVersion.Id);

            // Concept does not exist in the database, so cannot update it.
            if (oldConceptVersion == null)
            {
                return null;
            }

            // Readonly fields
            newConceptVersion.Created = oldConceptVersion.Created;
            newConceptVersion.ExternalId = oldConceptVersion.ExternalId;
            newConceptVersion.MediaIds = oldConceptVersion.MediaIds;
            newConceptVersion.AuthorName = oldConceptVersion.AuthorName;
            newConceptVersion.AuthorEmail = oldConceptVersion.AuthorEmail;
            newConceptVersion.GroupId = oldConceptVersion.GroupId;

            try
            {
                _conceptRepository.Update(newConceptVersion);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not update concept.");
                return viewModel;
            }

            // Updating media for concept
            var toBeDeleted = oldConceptVersion.Media
                .Where(x => !dto.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                .Select(x => x.Id)
                .ToList();
            var toBeInserted = dto.Media
                .Where(x => !oldConceptVersion.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                .ToList();

            try
            {
                if (toBeDeleted.Count > 0)
                    _conceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(oldConceptVersion.Id, toBeDeleted);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not delete medias for concept.");
                return viewModel;
            }

            try
            {
                _conceptMediaRepository.InsertMediaForConcept(oldConceptVersion.Id, toBeInserted);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not insert media on concept.");
                return viewModel;
            }

            viewModel.Data = Mapper.Map<ConceptDto>(_conceptRepository.GetById(dto.Id));
            return viewModel;
        }

        public Response CreateConcept(CreateConceptDto newConcept, UserInfo userInfo)
        {
            var viewModel = new Response();
            var concept = Mapper.Map<Concept>(newConcept);
            var media = new List<ConceptMedia>();

            // Readonly fields
            if (!string.IsNullOrEmpty(userInfo.Email))
                concept.AuthorEmail = userInfo.Email;
            if (!string.IsNullOrEmpty(userInfo.FullName))
                concept.AuthorName = userInfo.FullName;

            try
            {
                concept = _conceptRepository.Insert(concept);
                media = _conceptMediaRepository.InsertMediaForConcept(concept.Id, newConcept.Media);
            }
            catch(Exception e)
            {
                return viewModel;
            }

            concept.Media = media.Select(x => x.Media).ToList();
            viewModel.Data = Mapper.Map<ConceptDto>(concept);

            return viewModel;
        }

        public Response ArchiveConcept(int id, string usersEmail)
        {
            var updatedConcept = _conceptRepository.GetById(id);
            if (updatedConcept == null)
                return null;

            
            var viewModel = new Response();
            var inactiveStatus = _statusRepository.GetByName(Status.STATUS_ARCHVIED);
            if (inactiveStatus == null)
            {
                viewModel.Errors.TryAddModelError("errorMessage", $"Did not found \"{Status.STATUS_ARCHVIED}\" status");
                return viewModel;
            }

            updatedConcept.Status = inactiveStatus;
            updatedConcept.StatusId = inactiveStatus.Id;
            updatedConcept.DeletedBy = usersEmail;

            try
            {
                viewModel.Data = Mapper.Map<ConceptDto>(_conceptRepository.Update(updatedConcept));
            }
            catch (Exception)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not delete concept.");
                return viewModel;
            }

            return viewModel;
        }
    }
}
