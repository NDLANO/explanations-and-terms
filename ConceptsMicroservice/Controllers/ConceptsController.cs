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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    //[Authorize]
   // [Authorize(Policy = "RequireElevatedRights")]
    public class ConceptController : ControllerBase
    {
        private readonly IConceptService _service;
        private readonly IConfiguration _config;

        public ConceptController(IConceptService service)
        {
            _service = service;
            //_config = config;
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
        public async  Task<ActionResult<Response>> GetAll()
        {
            Models.Response conceptToBeDeleted = _service.GetConceptById(73);
            string scope = ReturnScope(User);
            string test = await ReturnClaimEmail();
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
        public async Task<ActionResult<Response>> UpdateConcept([Required][FromBody]Concept concept)
        {
            string scope = ReturnScope(User);
            string usersEmail = await ReturnClaimEmail();
            var canUserUpdate = (scope.Contains("concept-test:write") && 
                                 usersEmail.Equals(concept.AuthorName.ToLower())) || 
                                scope.Contains("concept-test:admin");
            if (canUserUpdate)
            {
                //do some thing...
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
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Authorize(Policy = "concept-test:admin")]
        [Authorize(Policy = "concept-test:write")]
        public async Task<ActionResult<Response>> CreateConcept([Required][FromBody]Concept concept)
        {
            string scope = ReturnScope(User);
            string usersEmail = await ReturnClaimEmail();
            bool userCanPost = scope.Contains("concept:write") ||
                               scope.Contains("concept:admin");
            if (userCanPost)
            {
                if (concept == null)
                {
                    var errors = new ModelStateDictionary();
                    errors.TryAddModelError("concept", "Concept is required");
                    return BadRequest(new ModelStateErrorResponse(errors));
                }

                if (!ModelState.IsValid)
                    return BadRequest(new ModelStateErrorResponse(ModelState));

                concept.AuthorName = usersEmail;
                var viewModel = _service.CreateConcept(concept);
                if (viewModel == null)
                    return BadRequest(new ModelStateErrorResponse(ModelState));

                if (viewModel.HasErrors())
                    return BadRequest(viewModel);

                return Ok(viewModel);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "concept-test:admin")]
        [Authorize(Policy = "concept-test:write")]
        //[Authorize(Policy = "RequireElevatedRights")]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            string token = await HttpContext.GetTokenAsync("access_token");
            string scope = ReturnScope(User);
            string usersEmail = await ReturnClaimEmail();
            Models.Response conceptToBeDeleted = _service.GetConceptById(id);
            var concept = conceptToBeDeleted.Data as Concept;
            var canUserDelete = concept != null && ((scope.Contains("concept-test:write") && 
                                                     usersEmail.Equals(concept.AuthorName.ToLower())) ||
                                                    scope.Contains("concept-test:admin"));
            if (canUserDelete)
            {
                var viewModel = _service.ArchiveConcept(id, usersEmail);
                if (viewModel == null)
                    return NotFound();

                if (viewModel.HasErrors())
                    return BadRequest(viewModel);
            }
            else
            {
               // StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized);
                return Unauthorized();
            }
            return NoContent();
        }

        #endregion
        #region check role
        private string ReturnScope(ClaimsPrincipal user)
        {
            string scopeValue = "";
            IEnumerable<Claim> scope = user.Claims.Where(c => c.Type.ToLower() == "scope");
            scopeValue = scope.First().Value;
            return scopeValue;
        }

        private async Task<string>  ReturnClaimEmail()
        {
            //auth0Domain should retrieve from appsettings
            string auth0Domain = "ndla.eu.auth0.com";
            string token = await HttpContext.GetTokenAsync("access_token");
            Auth0.AuthenticationApi.AuthenticationApiClient test =
                new AuthenticationApiClient(auth0Domain);
            Auth0.AuthenticationApi.Models.UserInfo authenticatedUser = await test.GetUserInfoAsync(token);

            return authenticatedUser.Email;
        }
        #endregion
    }
}
