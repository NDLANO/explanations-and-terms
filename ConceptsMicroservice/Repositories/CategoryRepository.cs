/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.Linq;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models;

namespace ConceptsMicroservice.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ConceptsContext _context;
        public CategoryRepository(ConceptsContext context)
        {
            _context = context;
        }

        public List<MetaCategory> GetRequiredCategories()
        {
            return _context.Categories.Where(x => x.IsRequired).ToList();
        }

        public List<MetaCategory> GetAll()
        {
            return _context.Categories.ToList();
        }

        public MetaCategory GetById(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }
    }
}