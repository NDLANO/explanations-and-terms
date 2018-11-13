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

        /// <summary>
        /// Checks whether the concepts has:
        /// A valid status, only valid meta objects and all required meta objects.
        /// </summary>
        /// <param name="concept"></param>
        /// <returns></returns>
        public Response ValidateConcept(Concept concept)
        {
            var viewModel = new Response();

            // The concept must have metadata, and they must be valid objects that exists in the database.
            if (!_metaRepository.MetaObjectsExists(concept.MetaIds))
            {
                viewModel.Errors.TryAddModelError("Meta",
                    $"Some of the metadata does not exists. It must exists before assigning it to concept.");
            }

            // The concept requires som metadata
            var requiredCategories = new List<string> {"Licence", "Language"};
            foreach (var category in requiredCategories)
            {
                var meta = _metaRepository.SearchForMetadata(new MetaSearchQuery {Category = category});
                if (meta == null)
                {
                    viewModel.Errors.TryAddModelError("Meta",
                        $"Some of the metadata does not exists. It must exists before assigning it to concept.");
                }
            }

            // Concept must have a valid status
            var status = _statusRepository.GetById(concept.StatusId);
            if (status == null)
            {
                viewModel.Errors.TryAddModelError("Status",
                    $"Status does not exist.");
            }

            return viewModel;
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

            var validationModel = ValidateConcept(newConceptVersion);
            if (validationModel.HasErrors())
                return validationModel;

            // Readonly fields
            newConceptVersion.Created = oldConceptVersion.Created;
            newConceptVersion.ExternalId = oldConceptVersion.ExternalId;
            newConceptVersion.GroupId = oldConceptVersion.GroupId;

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

            var validationModel = ValidateConcept(newConcept);
            if (validationModel.HasErrors())
                return validationModel;

            try
            {
                viewModel.Data = _conceptRepository.Insert(newConcept);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("Concept", "An database error has occured. Could not insert concept.");
            }

            return viewModel;
        }

        public Response SetStatusForConcept(int id, string statusName)
        {
            var updatedConcept = _conceptRepository.GetById(id);
            if (updatedConcept == null)
                return null;

            
            var viewModel = new Response();
            var status = _statusRepository.GetByName(statusName);
            if (status == null)
            {
                viewModel.Errors.TryAddModelError("Status", $"Did not found \"{statusName}\" status");
                return viewModel;
            }

            updatedConcept.Status = status;
            updatedConcept.StatusId = status.Id;

            try
            {
                viewModel.Data = _conceptRepository.Update(updatedConcept);
            }
            catch (Exception)
            {
                viewModel.Errors.TryAddModelError("Concept", "An database error has occured. Could not update concept status.");
            }

            return viewModel;
        }
    }
}
