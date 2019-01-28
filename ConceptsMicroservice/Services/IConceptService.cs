/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;

namespace ConceptsMicroservice.Services
{
    public interface IConceptService
    {
        Response SearchForConcepts(ConceptSearchQuery query);
        Response GetConceptById(int id);
        Response GetAllConcepts();
        Response GetAllConceptTitles(string language);
        Response UpdateConcept(UpdateConceptDto newConceptVersion);
        Response CreateConcept(CreateConceptDto newConcept);
        Response ArchiveConcept(int id, string usersEmail);
    }
}
