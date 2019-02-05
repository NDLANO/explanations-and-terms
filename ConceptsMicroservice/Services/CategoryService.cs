/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
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
            try
            {
                return new Response
                {
                    Data = _categoryRepository.GetAll()
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Response GetCategoryById(int id)
        {
            try
            {
                return new Response
                {
                    Data = _categoryRepository.GetById(id)
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}