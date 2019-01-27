/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.Search;

namespace ConceptsMicroservice.Repositories
{
    public interface IConceptRepository
    {
        List<Concept> SearchForConcepts(ConceptSearchQuery query);
        Concept GetById(int id);
        List<Concept> GetAll();
        Concept Update(Concept updated);
        Concept Insert(Concept inserted);
        List<string> GetAllTitles(string language);
    }
}
