/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ConceptController : ControllerBase
    {
        private readonly IConceptService _service;

        public ConceptController(IConceptService service)
        {
            _service = service;
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
        public ActionResult<Response> UpdateConcept([Required][FromBody]Concept concept)
        {
            if (concept == null || !ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            var viewModel = _service.UpdateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);
        }

        [HttpPost]
        public ActionResult<Response> CreateConcept([Required][FromBody]Concept concept)
        {
            if (concept == null || !ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            var viewModel = _service.CreateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public ActionResult<Response> DeleteConcept(int id)
        {
            var viewModel = _service.ArchiveConcept(id);
            if (viewModel == null)
                return NotFound();

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return NoContent();
        }

        #endregion
    }
}
