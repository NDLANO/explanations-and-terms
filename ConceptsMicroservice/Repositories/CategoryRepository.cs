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
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Context.ConceptsContext _context;
        public CategoryRepository(Context.ConceptsContext context)
        {
            _context = context;
        }

        private IQueryable<MetaCategory> TableWithAllNestedObjects()
        {
            return _context.Categories
                .Include(x => x.Language)
                .Include(x => x.TypeGroup);
        }

        public List<MetaCategory> GetAll(BaseListQuery query)
        {
            var allCategories = TableWithAllNestedObjects()
                .Where(x => x.Language.Abbreviation.Equals(query.Language));

            var totalItems = allCategories.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / query.PageSize));

            if (query.Page > totalPages)
                query.Page = 1;

            var categories = allCategories
                .OrderBy(x => x.Id)
                .Skip(query.PageSize * (query.Page - 1))
                .Take(query.PageSize)
                .ToList();
            categories.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });


            return categories;
        }

        public MetaCategory GetById(int id)
        {
            return TableWithAllNestedObjects()
                .FirstOrDefault(x => x.Id == id);
        }

        public List<MetaCategory> GetRequiredCategories(string language)
        {
            return TableWithAllNestedObjects()
                .Where(x => x.IsRequired && x.Language.Abbreviation.Equals(language)).ToList();
        }
    }
}