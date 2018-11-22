/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services.Validation
{
    public class ConceptValidationService : IConceptValidationService
    {
        private readonly IStatusRepository _statusRepository;

        public ConceptValidationService(IStatusRepository status)
        {
            _statusRepository = status;
        }
        public bool StatusIdIsValidId(int id)
        {
            return _statusRepository.GetById(id) != null;
        }
    }
}
