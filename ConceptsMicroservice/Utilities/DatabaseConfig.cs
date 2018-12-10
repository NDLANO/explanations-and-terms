/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Utilities
{
    public class DatabaseConfig : IDatabaseConfig
    {
        private readonly IConfigHelper _configHelper;

        public DatabaseConfig(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }
        public string GetConnectionString()
        {
            var host = _configHelper.GetVariable("DB_HOST");
            var database = _configHelper.GetVariable("DB_DATABASE");
            var user = _configHelper.GetVariable("DB_USER");
            var password = _configHelper.GetVariable("DB_PASSWORD");
            return $"Host={host};Database={database};Username={user};Password={password};";
        }
    }
}
