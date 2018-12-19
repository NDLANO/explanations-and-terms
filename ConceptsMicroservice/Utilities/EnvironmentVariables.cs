/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Utilities
{
    public class EnvironmentVariables
    {
        public static readonly string Auth0Domain = "AUTH0_DOMAIN";
        public static readonly string Auth0Audience = "AUTH0_AUDIENCE";
        public static readonly string Auth0ScopeConceptWrite = "AUTH0_SCOPE__CONCEPT_WRITE";
        public static readonly string Auth0ScopeConceptAdmin = "AUTH0_SCOPE__CONCEPT_ADMIN";


        public static readonly string DatabaseHost = "DB_HOST";
        public static readonly string DatabasePassword = "DB_PASSWORD";
        public static readonly string DatabaseUsername = "DB_USER";
        public static readonly string DatabaseName = "DB_DATABASE";
    }
}
