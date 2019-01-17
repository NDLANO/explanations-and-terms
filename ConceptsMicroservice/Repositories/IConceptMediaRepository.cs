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
        List<ConceptMedia> GetMediaForConcept(Concept concept);
        ConceptMedia Insert(Concept concept, string externalId, int mediaType);
        bool DeleteMediaForConcept(Concept concept, List<ConceptMedia> conceptMedia);
        List<ConceptMedia> InsertMediaForConcept(Concept concept, List<MediaWithMediaType> conceptMedia);
    }
}
