/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.Utilities
{
    public class DatabaseConfig : IDatabaseConfig
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public DatabaseConfig(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        public string GetConnectionString()
        {
            if (_env != null && _env.IsDevelopment())
                return _config.GetConnectionString("ConceptsDatabase");

            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var database = Environment.GetEnvironmentVariable("DB_DATABASE");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            return $"Host={host};Database={database};Username={user};Password={password};";
        }
    }
}
