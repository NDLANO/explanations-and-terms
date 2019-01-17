/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using ConceptsMicroservice.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Controllers
{
    [ApiController]
    [Route("concepts/api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
        protected StatusCodeResult InternalServerError()
        {
            return new InternalServerErrorObjectResult();
        }
    }
}
