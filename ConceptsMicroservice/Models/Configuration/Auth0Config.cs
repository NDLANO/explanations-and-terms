/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Models.Configuration
{
    public class Auth0Config
    {
        public string Domain{ get; set; }
        public string Audience { get; set; }
        public Auth0Scopes Scope { get; set; }
        public string DomainUrl => $"https://{Domain}";
    }
}
