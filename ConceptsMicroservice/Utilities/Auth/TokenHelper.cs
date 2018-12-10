/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using ConceptsMicroservice.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.Utilities.Auth
{
    public class TokenHelper: ITokenHelper
    {
        private readonly IConfiguration _config;

        public TokenHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> ReturnClaimEmail(HttpContext context)
        {
            var auth0Domain = _config["Auth0:Domain"];
            var token = await context.GetTokenAsync("access_token");
            AuthenticationApiClient authApiClient =
                new AuthenticationApiClient(auth0Domain);
            var authenticatedUser = await authApiClient.GetUserInfoAsync(token);

            return authenticatedUser.Email;
        }

        public string ReturnScope(ClaimsPrincipal user)
        {
            var scopeValue = "";
            var scope = user.Claims.FirstOrDefault(c => c.Type.ToLower() == "scope");
            if (scope != null)
                scopeValue = scope.Value;

            return scopeValue;
        }

        public async Task<string> ReturnToken(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");
            return token;
        }
    }
}
