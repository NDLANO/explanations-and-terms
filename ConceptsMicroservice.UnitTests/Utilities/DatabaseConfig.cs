/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.IO;
using ConceptsMicroservice.Utilities;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.UnitTests
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public string GetConnectionString()
        {
            var fileName = "appsettings.test.json";
            var file = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (File.Exists(file))
            {

                var connectionString = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(fileName, optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build().GetConnectionString("ConceptsDatabase");
                if (!string.IsNullOrEmpty(connectionString))
                    return connectionString;
            }

            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var database = Environment.GetEnvironmentVariable("DB_DATABASE");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            return $"Host={host};Database={database};Username={user};Password={password};";
        }
    }
}
