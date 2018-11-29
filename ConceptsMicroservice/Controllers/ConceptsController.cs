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
    [Authorize]
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
        public async Task<ActionResult<Response>> UpdateConcept([Required][FromBody]Concept concept)
        {
            string scope = ReturnScope(User);
            string usersEmail = await ReturnClaimEmail();
            var canUserUpdate = (scope.Contains("concept-test:write") && 
                                 usersEmail.Equals(concept.Author.ToLower())) || 
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
        [Authorize("concept-test:admin")]
        [Authorize("concept-write")]
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

                concept.Author = usersEmail;
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
        [Authorize("concept-test:admin")]
        [Authorize("concept-write")]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            string scope = ReturnScope(User);
            string usersEmail = await ReturnClaimEmail();
            Models.Response conceptToBeDeleted = _service.GetConceptById(id);
            var concept = conceptToBeDeleted.Data as Concept;
            var canUserDelete = concept != null && ((scope.Contains("concept-test:write") && 
                                                     usersEmail.Equals(concept.Author.ToLower())) ||
                                                    scope.Contains("concept-test:admin"));
            if (canUserDelete)
            {
                var viewModel = _service.ArchiveConcept(id);
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
            //token should retrieve from httprequest header
            //string token = await HttpContext.GetTokenAsync("access_token");
            string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9FSTFNVVU0T0RrNU56TTVNekkyTXpaRE9EazFOMFl3UXpkRE1EUXlPRFZDUXpRM1FUSTBNQSJ9.eyJodHRwczovL25kbGEubm8vbmRsYV9pZCI6Ii0xak4yOGpob3VTT19XN1d5c1JJU1lPbyIsImh0dHBzOi8vbmRsYS5uby91c2VyX25hbWUiOiJuYXNzZXIgcmFoYmFyaSIsImh0dHBzOi8vbmRsYS5uby9jbGllbnRfaWQiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsImlzcyI6Imh0dHBzOi8vbmRsYS5ldS5hdXRoMC5jb20vIiwic3ViIjoiZ29vZ2xlLW9hdXRoMnwxMTQzMzI3ODM4MTI1NDc5MjI4MDMiLCJhdWQiOlsibmRsYV9zeXN0ZW0iLCJodHRwczovL25kbGEuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU0MzQ3NjUwMCwiZXhwIjoxNTQzNDgzNzAwLCJhenAiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsInNjb3BlIjoiY29uY2VwdC10ZXN0OmFkbWluIGNvbmNlcHQtdGVzdDp3cml0ZSBvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.JB3AUC036h9bGLbyrVBNYRpRyK0oSgohIuJVwAj6bk3PZ5bW0YDPdXjMdvUJ-1FH38Ap4cnxaKP1Y6Ng7a8LAhph9R7RW6uDjtmVrAxMq8AWn3unELF7H31GWQMwpVihRis8V5AS0jXWxnxC_B5heJDBn7OmZSGtsm1b3oC6z_E_1hFedjYmvwsHogUtOlHBc_W4buoc_V8I8T8z4VoIPECxgYBz9lFsdW2YhbwH5MtSLP8sWH3eLlZp0M4hUDjfJB5fhQJ0x0RsiswVt_RdtpZh_R5D1XaNnGBkgtynHLj_tOtmUsoKfhjIaNQN4l9MXtodNYBbAMNxnnapyC8MRw";
            Auth0.AuthenticationApi.AuthenticationApiClient test =
                new AuthenticationApiClient(auth0Domain);
            Auth0.AuthenticationApi.Models.UserInfo authenticatedUser = await test.GetUserInfoAsync(token);

            return authenticatedUser.Email;
        }
        #endregion
    }
}
