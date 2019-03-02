/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using ConceptsMicroservice.Models.DTO;

namespace ConceptsMicroservice.Services.Validation
{
    public interface IConceptValidationService
    {
        bool StatusIdIsValidId(int id);
        List<int> MetaIdsDoesNotExistInDatabase(List<int> ids);
        List<int> MediaTypesNotExistInDatabase(List<MediaWithMediaType> ids, string language);
        List<string> GetMissingRequiredCategories(List<int> metaIds, string language);
    }
}
