/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Utilities
{
    
    public class InternalServerErrorObjectResult : StatusCodeResult
    {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:ConceptsMicroservice.Utilities.InternalServerErrorObjectResult" /> instance.
        /// </summary>
        public InternalServerErrorObjectResult() : base(StatusCodes.Status500InternalServerError)
        {
        }
    }
}
