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
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using NSwag.Annotations;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService _service;
        public MetadataController(IMetadataService service)
        {
            _service = service;
        }
        /// <summary>
        /// Find all metadata.
        /// </summary>
        /// <remarks>
        /// Returns a list of all the metadata.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<MetaData>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.BadRequest, null, Description = "Bad request")]
        [HttpGet]
        public ActionResult<Response> GetAll()
        {
            var meta = _service.GetAll();
            if (meta != null)
                return Ok(meta);

            return BadRequest();
        }

        /// <summary>
        /// Search for metadata.
        /// </summary>
        /// <remarks>
        /// Returns a list of metadata.
        /// </remarks>
        /// <param name="query"></param>
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<MetaData>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.BadRequest, null, Description = "Bad request")]
        [Route("[action]")]
        [HttpGet]
        public ActionResult<List<MetaData>> Search([FromQuery] MetaSearchQuery query = null)
        {
            var meta = _service.SearchForMetadata(query);
            if (meta != null)
                return Ok(meta);

            return BadRequest();
        }

        /// <summary>
        /// Fetch specified metadata.
        /// </summary>
        /// <remarks>
        /// Returns a single metadata.
        /// </remarks>
        /// <param name="id">Id of the metadata that is to be fetched.</param>
        [SwaggerResponse(HttpStatusCode.OK, typeof(MetaData), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "Not found")]
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
