﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
 
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1")]
    public class StatusController : BaseController
    {
        private readonly IStatusService _service;
        public StatusController(IStatusService service)
        {
            _service = service;
        }

        /// <summary>
        /// Find all status.
        /// </summary>
        /// <remarks>
        /// Returns a list of all the status.
        /// </remarks>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingDTO<StatusDTO>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        public ActionResult<Response> GetAllStatus([FromQuery] BaseListQuery query)
        {
            var status = _service.GetAllStatus(query);
            if (status != null)
                return Ok(status);

            return InternalServerError();
        }
    }
}
