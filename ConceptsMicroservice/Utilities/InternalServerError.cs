/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using ConceptsMicroservice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Utilities
{
    
    public class InternalServerErrorObjectResult : ObjectResult
    {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:ConceptsMicroservice.Utilities.InternalServerErrorObjectResult" /> instance.
        /// </summary>
        public InternalServerErrorObjectResult(Response error) : base(error.RenderedErrors)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    public class InternalServerErrorStatusResult : StatusCodeResult
    {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:ConceptsMicroservice.Utilities.InternalServerErrorObjectResult" /> instance.
        /// </summary>
        public InternalServerErrorStatusResult() : base(StatusCodes.Status500InternalServerError)
        {
        }
    }
}
