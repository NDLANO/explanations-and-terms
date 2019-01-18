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

        public Response GetAllConceptTitles()
        {
            try
            {
                return new Response
                {
                    Data = _conceptRepository.GetAllTitles()
                };
            }
            catch
            {
                return null;
            }
        }

        public Response UpdateConcept(Concept newConceptVersion)
        {
            var viewModel = new Response();
            var oldConceptVersion = _conceptRepository.GetById(newConceptVersion.Id);

            // Concept does not exist in the database, so cannot update it.
            if (oldConceptVersion == null)
            {
                viewModel.Errors.TryAddModelError("Concept", "Object does not exists.");
                return viewModel;
            }
            

            // Readonly fields
            newConceptVersion.Created = oldConceptVersion.Created;
            newConceptVersion.ExternalId = oldConceptVersion.ExternalId;

            try
            {
                viewModel.Data = _conceptRepository.Update(newConceptVersion);
            }
            catch (Exception)
            {
                viewModel.Errors.TryAddModelError("Concept", "An database error has occured. Could not update concept.");
            }
            
            return viewModel;
        }

        public Response CreateConcept(CreateOrUpdateConcept newConcept)
        {
            var viewModel = new Response();

            try
            {
                var concept = _mapper.Map<Concept>(newConcept);

                concept = _conceptRepository.Insert(concept);
                var media = _conceptMediaRepository.InsertMediaForConcept(concept, newConcept.Media);

                concept.Media = media.Select(x => x.Media).ToList();
                viewModel.Data = concept;
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not insert concept.");
            }


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
                viewModel.Errors.TryAddModelError("Status", $"Did not found \"{Status.STATUS_ARCHVIED}\" status");
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
                viewModel.Errors.TryAddModelError("Concept", "An database error has occured. Could not delete concept.");
            }

            return viewModel;
        }
    }
}
