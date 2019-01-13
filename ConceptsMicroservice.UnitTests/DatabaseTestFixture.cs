/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.UnitTests.Mock;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConceptsMicroservice.UnitTests
{
    
    public class DatabaseTestFixture : IClassFixture<Mock.MockDatabase>, IDisposable
    {
        private readonly IMockDatabase _database;
        public DatabaseTestFixture()
        {
            _database = new MockDatabase();
            _database.DeleteAllRowsInAllTables();
        }


        public void Dispose()
        {
            _database?.Dispose();
        }
    }
}
