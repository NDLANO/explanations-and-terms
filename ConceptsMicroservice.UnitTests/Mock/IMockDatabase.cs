/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;

namespace ConceptsMicroservice.UnitTests.Mock
{
    public interface IMockDatabase : IDisposable
    {
        ConceptsContext Context { get; set; }
        DatabaseConfig DatabaseConfig { get; set; }
        Language InsertLanguage(Language l = null);
        MetaCategory InsertCategory(MetaCategory mc);
        StatusDto InsertStatus(StatusDto ms);
        MetaData InsertMeta(MetaData m);
        Concept InsertConcept(Concept c);
        Concept CreateAndInsertAConcept();
        void DeleteAllRowsInAllTables();

    }
}