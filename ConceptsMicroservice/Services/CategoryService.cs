/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Response GetAllCategories()
        {
            return new Response
            {
                Data = _categoryRepository.GetAll()
            };
        }

        public Response GetCategoryById(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
                return null;
            return new Response
            {
                Data = category
            };
        }
    }
}