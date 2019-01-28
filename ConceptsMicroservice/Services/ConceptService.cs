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
using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class ConceptService : IConceptService
    {
        private readonly IConceptRepository _conceptRepository;
        private readonly IConceptMediaRepository _conceptMediaRepository;
        private readonly IStatusRepository _statusRepository;

        private readonly IMapper _mapper;

        public ConceptService(IConceptRepository concept,  IStatusRepository status, IConceptMediaRepository media, IMapper mapper)
        {
            _conceptRepository = concept;
            _statusRepository = status;
            _conceptMediaRepository = media;
            _mapper = mapper;
        }

        public Response SearchForConcepts(ConceptSearchQuery query)
        {
            try
            {
                return new Response
                {
                    Data = query == null ? _conceptRepository.GetAll() : _conceptRepository.SearchForConcepts(query)
                };
            }
            catch
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
                    Data = _conceptRepository.GetById(id)
                };
            }
            catch
            {
                return null;
            }
        }

        public Response GetAllConcepts()
        {
            try
            {
                return new Response
                {
                    Data = _conceptRepository.GetAll()
                };
            }
            catch
            {
                return null;
            }
        }

        public Response GetAllConceptTitles(string language)
        {
            try
            {
                return new Response
                {
                    Data = _conceptRepository.GetAllTitles(language)
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

            Concept concept;
            try
            {
                _conceptRepository.Update(newConceptVersion);
                concept = _conceptRepository.GetById(newConceptVersion.Id);
                
                // Updating media for concept
                var toBeDeleted = concept.Media
                    .Where(x => !dto.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                    .ToList();
                var toBeInserted = dto.Media
                    .Where(x => !concept.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                    .ToList();
                _conceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(concept, toBeDeleted);
                var media = _conceptMediaRepository.InsertMediaForConcept(concept, toBeInserted);

                concept.Media = media.Select(x => x.Media).ToList();
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not update concept.");
                return viewModel;
            }


            viewModel.Data = _mapper.Map<ConceptDto>(concept);
            return viewModel;
        }

        public Response CreateConcept(CreateConceptDto newConcept)
        {
            var viewModel = new Response();
            var concept = _mapper.Map<Concept>(newConcept);
            var media = new List<ConceptMedia>();

            try
            {
                concept = _conceptRepository.Insert(concept);
                media = _conceptMediaRepository.InsertMediaForConcept(concept, newConcept.Media);
            }
            catch
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
                viewModel.Data = _conceptRepository.Update(updatedConcept);
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
