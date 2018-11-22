/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Linq;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services.Validation
{
    public class ConceptValidationService : IConceptValidationService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ConceptValidationService(IStatusRepository status, IMetadataRepository meta, ICategoryRepository category)
        {
            _statusRepository = status;
            _metadataRepository = meta;
            _categoryRepository = category;
        }
        public bool StatusIdIsValidId(int id)
        {
            return _statusRepository.GetById(id) != null;
        }

        public List<int> MetaIdsDoesNotExistInDatabase(IEnumerable<int> ids)
        {
            var notExistingIds = new List<int>();

            if (ids == null)
                return notExistingIds;
            // TODO use GetByRangeIfMetaIds
            foreach (var id in ids)
            {
                var meta = _metadataRepository.GetById(id);
                if (meta == null)
                    notExistingIds.Add(id);
            }

            return notExistingIds;
        }

        public List<string> GetMissingRequiredCategories(List<int> metaIds)
        {
            var missingCategories = new List<string>();

            if (metaIds == null)
                metaIds = new List<int>();

            var requiredCategories = _categoryRepository.GetRequiredCategories();

            var metas = _metadataRepository.GetByRangeOfIds(metaIds);

            foreach (var requiredCategory in requiredCategories)
            {
                var category = metas.FirstOrDefault(x => x.Category.Id == requiredCategory.Id);
                if (category == null)
                    missingCategories.Add(requiredCategory.Name);
            }

            return missingCategories;
        }
    }
}
