/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Auth0.AuthenticationApi.Models;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1")]
    public class ConceptController : BaseController
    {
        private readonly IConceptService _service;
        private readonly ITokenHelper _tokenHelper;

        public ConceptController(IConceptService service, ITokenHelper tokenHelper)
        {
            _service = service;
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Search for concepts.
        /// </summary>
        /// <remarks>
        /// Returns a list of concepts.
        /// </remarks>
        /// <param name="query"></param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingDTO<ConceptDto>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Response> Search([FromQuery]ConceptSearchQuery query = null)
        {
            var concepts = _service.SearchForConcepts(query);
            if (concepts != null)
                return Ok(concepts);

            return InternalServerError();
        }

        #region CRUD

        /// <summary>
        /// Finds all concepts.
        /// </summary>
        /// <remarks>
        /// Returns a list of concepts.
        /// </remarks>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PagingDTO<ConceptDto>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        public ActionResult<Response> GetAll(BaseListQuery query)
        {
            var concepts = _service.GetAllConcepts(query);
            if (concepts != null)
                return Ok(concepts);

            return InternalServerError();
        }

        /// <summary>
        /// Fetch specified concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="id">Id of the concept that is to be fetched.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ConceptDto))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = null)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = null)]
        [HttpGet("{id}")]
        public ActionResult<Response> GetById(int id)
        {
            var concept = _service.GetConceptById(id);

            if (concept == null)
                return InternalServerError();
            if (concept.Data == null)
                return NotFound();
            
            return Ok(concept);
        }
        /// <summary>
        /// Fetch specified concept by id for imported Concepts.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="externalId">Id of the imported concept.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ConceptDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = null)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = null)]
        [HttpGet()]
        [Route("[action]")]
        public ActionResult<Response> GetByExternalId(string externalId)
        {
            var concept = _service.GetConceptByExternalId(externalId);

            if (concept == null)
                return InternalServerError();
            if (concept.Data == null)
                return NotFound();

            return Ok(concept);
        }


        /// <summary>
        /// Update the specified concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="concept">The concept to be updated with values.</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ConceptDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ModelStateErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ModelStateErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpPut]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]
        public ActionResult<Response> UpdateConcept([Required][FromBody]UpdateConceptDto concept)
        {
            if (concept == null)
            {
                var errors = new ModelStateDictionary();
                errors.TryAddModelError("errorMessage", "Concept is required");
                return BadRequest(new ModelStateErrorResponse(errors));
            }

            if (!ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            var viewModel = _service.UpdateConcept(concept);
            if (viewModel == null)
                return NotFound();

            if (viewModel.Data != null)
            {
                return Ok(viewModel);
            }

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return InternalServerError();

        }

        /// <summary>
        /// Create a concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="concept" >The concept to be created.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ConceptDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ModelStateErrorResponse))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]

        public async Task<ActionResult<Response>> CreateConcept([Required][FromBody]CreateConceptDto concept)
        {
            if (concept == null)
            {
                var errors = new ModelStateDictionary();
                errors.TryAddModelError("errorMessage", "Concept is required");
                return BadRequest(new ModelStateErrorResponse(errors));
            }

            if (!ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            UserInfo userInfo;
            try
            {
                userInfo = await _tokenHelper.GetUserInfo();
            }
            catch
            {
                return Unauthorized();
            }

            var viewModel = _service.CreateConcept(concept, userInfo);
            if (viewModel == null)
                return NotFound();

            if (viewModel.Data != null)
            {
                return Ok(viewModel);
            }

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return InternalServerError();
        }
        /// <summary>
        /// Update the specified concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="id">The id of the concepts to be deleted.</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{id}")]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]
        [ProducesResponseType((int)HttpStatusCode.NoContent, Type = typeof(void))]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            UserInfo userInfo;
            try
            {
                userInfo = await _tokenHelper.GetUserInfo();
            }
            catch
            {
                return Unauthorized();
            }

            var viewModel = _service.ArchiveConcept(id, userInfo.Email);
            if (viewModel == null)
                return NotFound();

            if (viewModel.HasErrors())
                return BadRequest(viewModel);
       
            return NoContent();
        }

        #endregion
    }
}
