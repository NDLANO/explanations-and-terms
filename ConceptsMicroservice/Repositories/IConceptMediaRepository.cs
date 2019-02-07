/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;

namespace ConceptsMicroservice.Repositories
{
    public interface IConceptMediaRepository
    {
        bool Delete(ConceptMedia relation);
        List<ConceptMedia> GetMediaForConcept(int conceptId);
        ConceptMedia Insert(int conceptId, MediaWithMediaType mediaType);
        bool DeleteConnectionBetweenConceptAndMedia(int conceptId, IEnumerable<int> mediaToBeDeleted);
        List<ConceptMedia> InsertMediaForConcept(int conceptId, List<MediaWithMediaType> conceptMedia);
    }
}
