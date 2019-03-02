/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1")]
    public class MediaTypeController : BaseController
    {
        private readonly IMediaTypeService _service;
        public MediaTypeController(IMediaTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Find all media types.
        /// </summary>
        /// <remarks>
        /// Returns a list of all the different media types.
        /// </remarks>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<MediaType>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        public ActionResult<Response> GetAllMediaTypes([FromQuery]BaseListQuery query)
        {
            var status = _service.GetAllMediaTypes(query);
            if (status != null)
                return Ok(status);

            return InternalServerError();
        }
    }
}
