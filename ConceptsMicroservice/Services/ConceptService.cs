/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Linq;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class ConceptService : IConceptService
    {
        private readonly IConceptRepository _conceptRepository;
        private readonly IMetadataRepository _metaRepository;
        private readonly IStatusRepository _statusRepository;

        public ConceptService(IConceptRepository concept, IMetadataRepository meta, IStatusRepository status)
        {
            _conceptRepository = concept;
            _metaRepository = meta;
            _statusRepository = status;
        }

        public Response SearchForConcepts(ConceptSearchQuery query)
        {
            return new Response
            {
                Data = query == null ? _conceptRepository.GetAll() : _conceptRepository.SearchForConcepts(query)
            };
        }

        public Response GetConceptById(int id)
        {
            var concept = _conceptRepository.GetById(id);
            if (concept == null)
                return null;
            return new Response
            {
                Data = concept
            };
        }

        public Response GetAllConcepts()
        {
            return new Response
            {
                Data = _conceptRepository.GetAll()
            };
        }

        public Response GetAllConceptTitles()
        {
            return new Response
            {
                Data = _conceptRepository.GetAllTitles()
            };
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

        public Response CreateConcept(Concept newConcept)
        {
            var viewModel = new Response();

            try
            {
                viewModel.Data = _conceptRepository.Insert(newConcept);
            }
            catch (Exception)
            {
                viewModel.Errors.TryAddModelError("Concept", "An database error has occured. Could not insert concept.");
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
            updatedConcept.Deleted_By = usersEmail;

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
