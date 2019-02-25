/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IUrlHelper urlHelper, IOptions<LanguageConfig> languageConfig) : base(mapper, urlHelper, languageConfig)
        {
            _categoryRepository = categoryRepository;
        }

        public Response GetAllCategories(BaseListQuery query)
        {
            if (query == null)
                query = BaseListQuery.DefaultValues(LanguageConfig.Default);
            query.SetDefaultValuesIfNotInitilized(LanguageConfig);

            try
            {
                var categories = _categoryRepository.GetAll(query);
                var results = Mapper.Map<List<MetaCategory>>(categories);

                return new Response
                {
                    Data = new MetaCategoryPagingDTO(results, query, UrlHelper.Action("Search", "Concept", query))
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