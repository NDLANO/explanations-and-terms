/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<Response> GetAllCategories()
        {
            var categories = _service.GetAllCategories();
            if (categories != null)
                return Ok(categories);

            return BadRequest();
        }
        [HttpGet("{id}")]
        public ActionResult<Response> GetCategoryById(int id)
        {
            var category = _service.GetCategoryById(id);
            if (category != null)
                return Ok(category);

            return NotFound();
        }
    }
}
