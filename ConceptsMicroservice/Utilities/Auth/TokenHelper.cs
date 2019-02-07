/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using ConceptsMicroservice.Models.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Utilities.Auth
{
    public class TokenHelper: ITokenHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Auth0Config _auth0Config;

        public TokenHelper(IOptions<Auth0Config> config, IHttpContextAccessor ca)
        {
            _auth0Config = config.Value;
            _httpContextAccessor = ca;
        }

        public async Task<UserInfo> GetUserInfo()
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var authApiClient = new AuthenticationApiClient(_auth0Config.Domain);
            var authenticatedUser = await authApiClient.GetUserInfoAsync(token);

            return authenticatedUser;
        }

        public string ReturnScope(ClaimsPrincipal user)
        {
            var scopeValue = "";
            var scope = user.Claims.FirstOrDefault(c => c.Type.ToLower() == "scope");
            if (scope != null)
                scopeValue = scope.Value;

            return scopeValue;
        }
    }
}
