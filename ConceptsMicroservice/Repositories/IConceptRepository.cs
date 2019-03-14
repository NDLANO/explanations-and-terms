/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.Search;

namespace ConceptsMicroservice.Repositories
{
    public interface IConceptRepository
    {
        List<Concept> SearchForConcepts(ConceptSearchQuery query);
        Concept GetById(int id);
        List<Concept> GetByGroupId(Guid id);
        List<Concept> GetAll(BaseListQuery query);
        Concept Update(Concept updated);
        Concept Insert(Concept inserted);
        Concept GetByExternalId(string externalId);
    }
}
