/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Net;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using ConceptsMicroservice.Services;

namespace ConceptsMicroservice.Controllers
{
    [ApiVersion("1")]
    public class CategoryController : BaseController
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<MetaCategory>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet]
        public ActionResult<Response> GetAllCategories()
        {
            var categories = _service.GetAllCategories();
            if (categories != null)
                return Ok(categories);

            return InternalServerError();
        }

        /// <summary>
        /// Fetch specified category.
        /// </summary>
        /// <remarks>
        /// Returns a single category.
        /// </remarks>
        /// <param name="id">Id of the category that is to be fetched.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MetaCategory))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(void))]
        [HttpGet("{id}")]
        public ActionResult<Response> GetCategoryById(int id)
        {
            var category = _service.GetCategoryById(id);

            if (category == null)
                return InternalServerError();
            if (category.Data == null)
                return NotFound();
            
            return Ok(category);
        }
    }
}
