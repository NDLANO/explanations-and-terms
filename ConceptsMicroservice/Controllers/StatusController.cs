/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _service;
        public StatusController(IStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<Response> GetAllStatus()
        {
            var status = _service.GetAllStatus();
            if (status != null)
                return Ok(status);

            return BadRequest();
        }
    }
}
