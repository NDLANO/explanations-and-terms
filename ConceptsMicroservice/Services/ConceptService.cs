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
    public class ConceptService : IConceptService
    {
        private readonly IConceptRepository _conceptRepository;
        private readonly IConceptMediaRepository _conceptMediaRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly LanguageConfig _languageConfig;
        private readonly IMapper _mapper;

        public ConceptService(IConceptRepository concept,  IStatusRepository status, IConceptMediaRepository media, IMapper mapper, IUrlHelper urlHelper, IOptions<LanguageConfig> languageConfig)
        {
            _conceptRepository = concept;
            _statusRepository = status;
            _conceptMediaRepository = media;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _languageConfig = languageConfig.Value;
        }

        public Response SearchForConcepts(ConceptSearchQuery query)
        {
            //Nasser 13.02
            try
            {
                var searchResult = query == null
                    ? null//_conceptRepository.GetAll()
                    : _conceptRepository.SearchForConcepts(query);
                return new Response
                {
                    Data = _mapper.Map<List<ConceptDto>>(searchResult)
                };
            }
            catch(Exception e)
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
                    Data = _mapper.Map<ConceptDto>(_conceptRepository.GetById(id))
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Response GetAllConcepts(BaseListQuery query)
        {
            if (query == null)
                query = BaseListQuery.DefaultValues(_languageConfig.Default);

            if (!_languageConfig.Supported.Contains(query.Language))
                query.Language = _languageConfig.Default;

            try
            {
                var concepts = _conceptRepository.GetAll(query);

                var res = new ConceptResultDTO
                {
                    PageSize = query.PageSize,
                    Page = query.Page,
                    Concepts = _mapper.Map<List<ConceptDto>>(concepts)
                };

                if (concepts.FirstOrDefault() != null)
                {
                    res.TotalItems = concepts.FirstOrDefault().TotalItems;
                    res.NumberOfPages = concepts.FirstOrDefault().NumberOfPages;
                }

                if (query.Page < res.NumberOfPages)
                {
                    query.Page += 1;
                    res.Next = _urlHelper.Action("GetAll", "Concept", query);
                }

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
            var newConceptVersion = _mapper.Map<Concept>(dto);
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

            viewModel.Data = _mapper.Map<ConceptDto>(_conceptRepository.GetById(dto.Id));
            return viewModel;
        }

        public Response CreateConcept(CreateConceptDto newConcept, UserInfo userInfo)
        {
            var viewModel = new Response();
            var concept = _mapper.Map<Concept>(newConcept);
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
            viewModel.Data = _mapper.Map<ConceptDto>(concept);

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
                viewModel.Data = _mapper.Map<ConceptDto>(_conceptRepository.Update(updatedConcept));
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
