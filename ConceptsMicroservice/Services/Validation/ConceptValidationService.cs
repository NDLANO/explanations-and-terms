/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services.Validation
{
    public class ConceptValidationService : IConceptValidationService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metadataRepository;

        public ConceptValidationService(IStatusRepository status, IMetadataRepository meta)
        {
            _statusRepository = status;
            _metadataRepository = meta;
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

            foreach (var id in ids)
            {
                var meta = _metadataRepository.GetById(id);
                if (meta == null)
                    notExistingIds.Add(id);
            }

            return notExistingIds;
        }
    }
}
