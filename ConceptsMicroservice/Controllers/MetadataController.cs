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

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1")]
    [Route("concepts/meta-api")]
    public class MetadataController : BaseController
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<MetaData>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        public ActionResult<Response> GetAll()
        {
            var meta = _service.GetAll();
            if (meta != null)
                return Ok(meta);

            return InternalServerError();
        }

        /// <summary>
        /// Search for metadata.
        /// </summary>
        /// <remarks>
        /// Returns a list of metadata.
        /// </remarks>
        /// <param name="query"></param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<MetaData>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [Route("[action]")]
        [HttpGet]
        public ActionResult<List<MetaData>> Search([FromQuery] MetaSearchQuery query = null)
        {
            var meta = _service.SearchForMetadata(query);
            if (meta != null)
                return Ok(meta);

            return InternalServerError();
        }

        /// <summary>
        /// Fetch specified metadata.
        /// </summary>
        /// <remarks>
        /// Returns a single metadata.
        /// </remarks>
        /// <param name="id">Id of the metadata that is to be fetched.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MetaData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet("{id}")]
        public ActionResult<Response> GetById(int id)
        {
            var meta = _service.GetById(id);

            if (meta == null)
                return InternalServerError();
            if (meta.Data == null)
                return NotFound();

            return Ok(meta);
        }
    }
}
