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
        private readonly ICategoryRepository _categoryRepository;

        public ConceptService(IConceptRepository concept, IMetadataRepository meta, IStatusRepository status, ICategoryRepository category)
        {
            _conceptRepository = concept;
            _metaRepository = meta;
            _statusRepository = status;
            _categoryRepository = category;
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

        private bool ContainsDuplicate<T>(IReadOnlyCollection<T> list)
        {
            return list.Count != list.Distinct().Count();
        }

        private bool StatusExistsInDatabase(int statusId)
        {
            var status = _statusRepository.GetById(statusId);
            return status != null;
        }

        private bool MetasExistInDatabase(IEnumerable<MetaData> metasFromDb, List<int> conceptMetaIds)
        {
            var metasAsIdList = metasFromDb.Select(x => x.Id).ToList();
            return metasAsIdList.All(conceptMetaIds.Contains) && metasAsIdList.Count == conceptMetaIds.Count;
        }

        private bool ContainsMultipleUniqueMetasOfSameCategory(IEnumerable<MetaData> metasFromDb)
        {
            var categories = metasFromDb.Where(x => !x.Category.ConceptCanHaveMultipleMeta).Select(x => x.Category.Id).ToList();
            return ContainsDuplicate(categories);
        }

        private bool ConceptContainsAllRequiredMetaCategories(List<MetaData> metasFromDb)
        {
            var requiredCategories = _categoryRepository.GetRequiredCategories();
            foreach (var requiredCategory in requiredCategories)
            {
                if (metasFromDb.FirstOrDefault(x => x.Category.Id == requiredCategory.Id) == null)
                    return false;
            }
            return true;
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

            if (ContainsDuplicate(concept.MetaIds))
            {
                viewModel.Errors.TryAddModelError("Meta", "Duplicate metas is not allowed.");
            }

            var metas = _metaRepository.GetByListOfIds(concept.MetaIds);

            if (!MetasExistInDatabase(metas, concept.MetaIds))
            {
                viewModel.Errors.TryAddModelError("Meta", "Some of the metadata does not exists in the database.");
            }

            if (!ConceptContainsAllRequiredMetaCategories(metas))
            {
                viewModel.Errors.TryAddModelError("Meta", "Did not contain all required metas for concept.");
            }

            if (ContainsMultipleUniqueMetasOfSameCategory(metas))
            {
                viewModel.Errors.TryAddModelError("Meta", "Contains multiple metas of same category, when concept only allows one meta of this specific category.");
            }

            if (!StatusExistsInDatabase(concept.StatusId))
            {
                viewModel.Errors.TryAddModelError("Status", "Status does not exist.");
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
