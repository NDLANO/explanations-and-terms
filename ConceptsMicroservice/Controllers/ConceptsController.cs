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
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwag.Annotations;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("2")]
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
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<Concept>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(void), Description = "Unknown error")]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Response> Search([FromQuery]ConceptSearchQuery query = null)
        {
            var concepts = _service.SearchForConcepts(query);
            if (concepts != null)
                return Ok(concepts);

            return InternalServerError();
        }

        /// <summary>
        /// Finds all concept titles.
        /// </summary>
        /// <remarks>
        /// Returns a list of concept titles.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<string>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(void), Description = "Unknown error")]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Response> AllTitles()
        {
            var concepts = _service.GetAllConceptTitles();
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
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<Concept>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, null, Description = "Unknown error")]
        [HttpGet]
        public ActionResult<Response> GetAll()
        {
            var concepts = _service.GetAllConcepts();
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
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<Concept>), Description = "OK")]
        [SwaggerResponse(HttpStatusCode.NotFound, null, Description = "If a concept with the specified id does not exist")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, null, Description = "Unknown error")]
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
        /// Update the specified concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="concept">The concept to be updated with values.</param>
        [SwaggerIgnore]
        [HttpPut]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]
        public ActionResult<Response> UpdateConcept([Required][FromBody]Concept concept)
        {
            if (concept == null)
            {
                var errors = new ModelStateDictionary();
                errors.TryAddModelError("concept", "Concept is required");
                return BadRequest(new ModelStateErrorResponse(errors));
            }

            if (!ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            var viewModel = _service.UpdateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);

        }

        /// <summary>
        /// Create a concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="concept" >The concept to be created.</param>
        [SwaggerIgnore]
        [HttpPost]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]
        public async Task<ActionResult<Response>> CreateConcept([Required][FromBody]Concept concept)
        {
            if (concept == null)
            {
                var errors = new ModelStateDictionary();
                errors.TryAddModelError("concept", "Concept is required");
                return BadRequest(new ModelStateErrorResponse(errors));
            }

            if (!ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            concept.AuthorEmail = await _tokenHelper.ReturnClaimEmail(HttpContext);
            concept.AuthorName = await _tokenHelper.ReturnClaimFullName(HttpContext);
            var viewModel = _service.CreateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);
        }
        /// <summary>
        /// Update the specified concept.
        /// </summary>
        /// <remarks>
        /// Returns a single concept.
        /// </remarks>
        /// <param name="id">The id of the concepts to be deleted.</param>
        [SwaggerIgnore]
        [HttpDelete("{id}")]
        [Authorize(Policy = "concept:admin")]
        [Authorize(Policy = "concept:write")]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            var usersEmail = await _tokenHelper.ReturnClaimEmail(HttpContext);
            
            var viewModel = _service.ArchiveConcept(id, usersEmail);
            if (viewModel == null)
                return NotFound();

            if (viewModel.HasErrors())
                return BadRequest(viewModel);
       
            return NoContent();
        }

        #endregion
    }
}
