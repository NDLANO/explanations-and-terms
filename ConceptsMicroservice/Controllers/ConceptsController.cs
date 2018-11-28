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

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
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
        public async  Task<ActionResult<Response>> GetAll()
        {
            //string token = await HttpContext.GetTokenAsync("access_token");
            Models.Response conceptToBeDeleted = _service.GetConceptById(73);
            string scope = ReturnScope();
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
            string scope = ReturnScope();
            string usersEmail = await ReturnClaimEmail();
            if (scope.Contains("concept-test:write") && usersEmail.Equals(concept.Author.ToLower()) || 
                scope.Contains("concept-test:admin"))
            {
                //do some thing...
            }
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
        public ActionResult<Response> CreateConcept([Required][FromBody]Concept concept)
        {
            bool isWriter = ReturnScope().Contains("concept:write") || 
                            ReturnScope().Contains("concept:admin");
            if (concept == null)
            {
                var errors = new ModelStateDictionary();
                errors.TryAddModelError("concept", "Concept is required");
                return BadRequest(new ModelStateErrorResponse(errors));
            }

            if (!ModelState.IsValid)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            var viewModel = _service.CreateConcept(concept);
            if (viewModel == null)
                return BadRequest(new ModelStateErrorResponse(ModelState));

            if (viewModel.HasErrors())
                return BadRequest(viewModel);

            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteConcept(int id)
        {
            string scope = ReturnScope();
            string usersEmail = await ReturnClaimEmail();
            Models.Response conceptToBeDeleted = _service.GetConceptById(id);
            var concept = conceptToBeDeleted.Data as Concept;
            if ((scope.Contains("concept:write") &&
                 usersEmail.Equals(concept.Author.ToLower())) ||
                scope.Contains("concept:admin"))
            {
                //implement youe logic here; } e
                var viewModel = _service.ArchiveConcept(id);
                if (viewModel == null)
                    return NotFound();

                if (viewModel.HasErrors())
                    return BadRequest(viewModel);
            }
            else
            {
                var returnSomeKindOf = "return some kind of 401";
            }

            //var viewModel = _service.ArchiveConcept(id);
            //if (viewModel == null)
            //    return NotFound();

            //if (viewModel.HasErrors())
            //    return BadRequest(viewModel);

            return NoContent();
        }

        #endregion
        #region check role
        private bool IsThisUserAllowed(string role, string email)
        {
            bool isAllowed = false;
            //do necessary  check for the roles/user in Access Token, Claims
            //string claimsString = "";
            //string test = "";
            var scopeClaim = User.Claims.Where(c => c.Type.ToLower() == "scope");
            var emailClaim = User.Claims.Where(c => c.Type.ToLower() == "email");
            var roleClaim = User.Claims.Where(c => c.Type.ToLower() == "role");
            if (emailClaim.ToString().ToLower() == email)
            {
                isAllowed = true;
            }
            return isAllowed;
        }

        private string ReturnScope()
        {
            string scopeValue = "";
            IEnumerable<Claim> scope = User.Claims.Where(c => c.Type.ToLower() == "scope");
            scopeValue = scope.First().Value;
            return scopeValue;
        }

        private async Task<string>  ReturnClaimEmail()
        {
            string emailValue = "";

            Auth0.AuthenticationApi.AuthenticationApiClient test  = 
                new AuthenticationApiClient("ndla.eu.auth0.com");
            Auth0.AuthenticationApi.Models.UserInfo thisUser = await test.GetUserInfoAsync(
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9FSTFNVVU0T0RrNU56TTVNekkyTXpaRE9EazFOMFl3UXpkRE1EUXlPRFZDUXpRM1FUSTBNQSJ9.eyJodHRwczovL25kbGEubm8vbmRsYV9pZCI6Ii0xak4yOGpob3VTT19XN1d5c1JJU1lPbyIsImh0dHBzOi8vbmRsYS5uby91c2VyX25hbWUiOiJuYXNzZXIgcmFoYmFyaSIsImh0dHBzOi8vbmRsYS5uby9jbGllbnRfaWQiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsImlzcyI6Imh0dHBzOi8vbmRsYS5ldS5hdXRoMC5jb20vIiwic3ViIjoiZ29vZ2xlLW9hdXRoMnwxMTQzMzI3ODM4MTI1NDc5MjI4MDMiLCJhdWQiOlsibmRsYV9zeXN0ZW0iLCJodHRwczovL25kbGEuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU0MzQxNTQxOCwiZXhwIjoxNTQzNDIyNjE4LCJhenAiOiJXVTBLcjRDRGtyTTB1TDl4WWVGVjRjbDlHYTF2QjNKWSIsInNjb3BlIjoiY29uY2VwdC10ZXN0OmFkbWluIGNvbmNlcHQtdGVzdDp3cml0ZSBvcGVuaWQgcHJvZmlsZSBlbWFpbCJ9.Y0Tv1Vkygy-r0YpIFbAdBqdkgGR07baP0EGTgQuQ3VupjuaE1BodkQtdlpy__z0svAq_5sRK-R7ZdVxLFjyjQ8kSrgr1VSKzqcsqw3eXUZv3J2_DfLWtIlU43DBwpBw_GXskud2ptyJnuW_gu8n7pJCcldmpZ6UmIvWj89q5TDRGp2jtUHJA2rCmudpzjYHwmVGVe-H4XeW283HsVNult6aaRmFZyEg0dfimgfVKnexA7MZsaDJUyDopIZvugUHQhC4i81zleP-7_0aFDkyE_F3e4oVr6E_Jhdg0NYH4DZDShHUU1QGemPjt5iKunPRnISvJmCkZ-4vEJ8s5wBu-uA");

            return thisUser.Email;
        }
        #endregion
    }
}
