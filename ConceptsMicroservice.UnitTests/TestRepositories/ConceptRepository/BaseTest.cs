/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.UnitTests.Helpers;
using ConceptsMicroservice.UnitTests.Mock;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories.ConceptRepository
{
    public class BaseTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected readonly IConceptRepository ConceptRepository;
        protected IMock Mock;

        public BaseTest()
        {
            Mock = new Mock.Mock();
            var config = Options.Create(ConfigHelper.GetApplicationConfiguration());

            ConceptRepository = new Repositories.ConceptRepository(Mock.Database.Context, config);
        }

        public void Dispose()
        {
            Mock.Dispose();
        }
    }
}
