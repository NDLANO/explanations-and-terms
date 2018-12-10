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
    public class ConfigHelper : IConfigHelper
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public ConfigHelper(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public string GetVariable(string key)
        {
            if (_env != null && _env.IsDevelopment())
                return _config[key];

            return Environment.GetEnvironmentVariable(key);
        }
    }
}
