/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.UnitTests.Helpers;
using ConceptsMicroservice.UnitTests.Mock;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories.ConceptRepository
{
    public class BaseTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected BaseListQuery BaseListQuery;
        protected readonly IConceptRepository ConceptRepository;
        protected IMock Mock;

        public BaseTest()
        {
            Mock = new Mock.Mock();
            var config = Options.Create(ConfigHelper.GetDatabaseConfiguration());
            var languageConfig = Options.Create(ConfigHelper.GetLanguageConfiguration());

            ConceptRepository = new Repositories.ConceptRepository(Mock.Database.Context, config, languageConfig);
            BaseListQuery = BaseListQuery.DefaultValues("en");
        }

        public void Dispose()
        {
            Mock.Dispose();
        }
    }
}
