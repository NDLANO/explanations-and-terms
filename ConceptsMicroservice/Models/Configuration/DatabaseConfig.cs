/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Models.Configuration
{
    public class DatabaseConfig
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public string ConnectionString => $"Host={Host};Database={Name};Username={User};Password={Password};";
    }
}
