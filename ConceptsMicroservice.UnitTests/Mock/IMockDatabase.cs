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
using ConceptsMicroservice.Utilities;

namespace ConceptsMicroservice.UnitTests.Mock
{
    public interface IMockDatabase : IDisposable
    {
        ConceptsContext Context { get; set; }
        IDatabaseConfig DatabaseConfig { get; set; }

        MetaCategory InsertCategory(MetaCategory mc);
        Status InsertStatus(Status ms);
        MetaData InsertMeta(MetaData m);
        Concept InsertConcept(Concept c);
        Concept CreateAndInsertAConcept();
        void DeleteAllRowsInAllTables();

    }
}