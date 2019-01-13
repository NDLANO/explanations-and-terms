/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.UnitTests.Helpers
{
    class ConfigHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static DatabaseConfig GetApplicationConfiguration()
        {
            var configuration = new DatabaseConfig();

            var iConfig = GetIConfigurationRoot();

            iConfig
                .GetSection("Database")
                .Bind(configuration);

            return configuration;
        }
    }
}
