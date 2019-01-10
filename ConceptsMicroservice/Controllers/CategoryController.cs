﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Net;
using ConceptsMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Services;
using NSwag.Annotations;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Find all categories used for metadata.
        /// </summary>
        /// <remarks>
        /// Returns a list of all the categories.
        /// </remarks>
        [SwaggerResponse(200, typeof(List<MetaCategory>), Description = "List of categories")]
        [SwaggerResponse(400, null, Description = "Empty response")]
        [HttpGet]
        public ActionResult<Response> GetAllCategories()
        {
            var categories = _service.GetAllCategories();
            if (categories != null)
                return Ok(categories);

            return BadRequest();
        }

        /// <summary>
        /// Fetch specified category.
        /// </summary>
        /// <remarks>
        /// Returns a single category.
        /// </remarks>
        /// <param name="id">Id of the category that is to be fetched.</param>
        [SwaggerResponse(200, typeof(MetaCategory), Description = "A single category")]
        [SwaggerResponse(401, null, Description = "Empty response")]
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
