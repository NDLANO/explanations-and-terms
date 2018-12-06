/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConceptsMicroservice.Utilities.Auth
{
    public interface ITokenHelper
    {
        Task<string> ReturnClaimEmail(HttpContext context);

        string ReturnScope(ClaimsPrincipal user);

        Task<string> ReturnToken(HttpContext context);
    }
}
