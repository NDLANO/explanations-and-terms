/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService _service;
        public MetadataController(IMetadataService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<Response> GetAll()
        {
            var meta = _service.GetAll();
            if (meta != null)
                return Ok(meta);

            return BadRequest();
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<List<MetaData>> Search([FromQuery] MetaSearchQuery query = null)
        {
            var meta = _service.SearchForMetadata(query);
            if (meta != null)
                return Ok(meta);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetById(int id)
        {
            var meta = _service.GetById(id);
            if (meta != null)
                return Ok(meta);

            return NotFound();
        }
    }
}
