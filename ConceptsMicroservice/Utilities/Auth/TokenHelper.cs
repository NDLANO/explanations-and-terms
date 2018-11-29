/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.Utilities.Auth
{
    public class TokenHelper: ITokenHelper
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public TokenHelper(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        public async Task<string> ReturnClaimEmail()
        {
            //auth0Domain should retrieve from appsettings
            string auth0Domain = _config["Auth0:Domain"];// "ndla.eu.auth0.com";
            //toke should retrieve from httprequest header
            //string token = await HttpContext.GetTokenAsync("access_token");
            string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9FSTFNVVU0T0RrNU56TTVNekkyTXpaRE9EazFOMFl3UXpkRE1EUXlPRFZDUXpRM1FUSTBNQSJ9.eyJodHRwczovL25kbGEubm8vbmRsYV9pZCI6Ii0xak4yOGpob3VTT19XN1d5c1JJU1lPbyIsImh0dHBzOi8vbmRsYS5uby91c2VyX25hbWUiOiJuYXNzZXIgcmFoYmFyaSIsImh0dHBzOi8vbmRsYS5uby9jbGllbnRfaWQiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsImlzcyI6Imh0dHBzOi8vbmRsYS5ldS5hdXRoMC5jb20vIiwic3ViIjoiZ29vZ2xlLW9hdXRoMnwxMTQzMzI3ODM4MTI1NDc5MjI4MDMiLCJhdWQiOlsibmRsYV9zeXN0ZW0iLCJodHRwczovL25kbGEuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU0MzQxNTQxOCwiZXhwIjoxNTQzNDIyNjE4LCJhenAiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsInNjb3BlIjoiY29uY2VwdC10ZXN0OmFkbWluIGNvbmNlcHQtdGVzdDp3cml0ZSBvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.Y0Tv1Vkygy-r0YpIFbAdBqdkgGR07baP0EGTgQuQ3VupjuaE1BodkQtdlpy__z0svAq_5sRK-R7ZdVxLFjyjQ8kSrgr1VSKzqcsqw3eXUZv3J2_DfLWtIlU43DBwpBw_GXskud2ptyJnuW_gu8n7pJCcldmpZ6UmIvWj89q5TDRGp2jtUHJA2rCmudpzjYHwmVGVe-H4XeW283HsVNult6aaRmFZyEg0dfimgfVKnexA7MZsaDJUyDopIZvugUHQhC4i81zleP-7_0aFDkyE_F3e4oVr6E_Jhdg0NYH4DZDShHUU1QGemPjt5iKunPRnISvJmCkZ-4vEJ8s5wBu-uA";

            Auth0.AuthenticationApi.AuthenticationApiClient test =
                new AuthenticationApiClient(auth0Domain);
            Auth0.AuthenticationApi.Models.UserInfo authenticatedUser = await test.GetUserInfoAsync(token);

            return authenticatedUser.Email;
        }

        public string ReturnScope(ClaimsPrincipal user)
        {
            string scopeValue = "";
            IEnumerable<Claim> scope = user.Claims.Where(c => c.Type.ToLower() == "scope");
            scopeValue = scope.First().Value;
            return scopeValue;
        }
    }
}
