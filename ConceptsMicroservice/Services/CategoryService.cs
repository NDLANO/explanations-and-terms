/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IUrlHelper urlHelper) : base(mapper, urlHelper)
        {
            _categoryRepository = categoryRepository;
        }

        public Response GetAllCategories(BaseListQuery query)
        {
            try
            {
                var categories = _categoryRepository.GetAll(query);
                var results = Mapper.Map<List<MetaCategoryDTO>>(categories);

                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = categories.FirstOrDefault().TotalItems;
                    numberOfPages = categories.FirstOrDefault().NumberOfPages;
                }
                catch { }

                return new Response
                {
                    Data = new PagingDTO<MetaCategoryDTO>(results, query, UrlHelper.Action("GetAllCategories", "Category", query), numberOfPages, totalItems)
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