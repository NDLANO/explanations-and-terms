/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    //[Authorize]
   // [Authorize(Policy = "RequireElevatedRights")]
    public class ConceptController : ControllerBase
    {
        private readonly IConceptService _service;
        private readonly ITokenHelper _tokenHelper;

        public ConceptController(IConceptService service, ITokenHelper tokenHelper)
        {
            _service = service;
            _tokenHelper = tokenHelper;
        }
        
        [HttpGet]
        [Route("[action]")]
        public ActionResult<Response> Search([FromQuery]ConceptSearchQuery query = null)
        {
            var concepts = _service.SearchForConcepts(query);
            if (concepts != null)
                return Ok(concepts);

            return BadRequest();
        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult<Response> AllTitles()
        {
            var concepts = _service.GetAllConceptTitles();
            if (concepts != null)
                return Ok(concepts);

            return BadRequest();
        }

        #region CRUD

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Response> GetAll()
        {
            var concepts = _service.GetAllConcepts();
            if (concepts != null)
                return Ok(concepts);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetById(int id)
        {
            var concept = _service.GetConceptById(id);
            if (concept != null)
                return Ok(concept);

            return NotFound();
        }

        [HttpPut]
        [Authorize(Policy = "concept-test:admin")]
        [Authorize(Policy = "concept-test:write")]
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
        

        [HttpPost]
        [Authorize(Policy = "concept-test:admin")]
        [Authorize(Policy = "concept-test:write")]
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

            concept.AuthorName = await _tokenHelper.ReturnClaimEmail(HttpContext);
            var viewModel = _service.CreateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "concept-test:admin")]
        [Authorize(Policy = "concept-test:write")]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            
            string usersEmail = await _tokenHelper.ReturnClaimEmail(HttpContext);
            
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
