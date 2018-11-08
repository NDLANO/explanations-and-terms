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
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        private readonly ConceptsContext _context;
        public MetadataRepository(ConceptsContext context)
        {
            _context = context;
        }

        public bool MetaObjectsExists(List<MetaData> meta)
        {
            if (meta == null || meta.Count == 0)
                return false;

            foreach (var data in meta)
            {
                var m = GetById(data.Id);
                if (m == null || m.Name != data.Name)
                    return false;
            }

            return true;
        }

        public List<MetaData> GetAll()
        {
            return _context.MetaData
                .Include(x => x.Category)
                .Include(x => x.Status)
                .ToList();
        }

        public MetaData GetById(int id)
        {
            return _context.MetaData
                .Include(x => x.Category)
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<MetaData> SearchForMetadata(MetaSearchQuery searchArgument)
        {
            var query = _context.MetaData.IncludeAll().AsQueryable();
            if (searchArgument == null)
                return query.ToList();

            var searchArgsHasName = !string.IsNullOrWhiteSpace(searchArgument.Name);
            var searchArgsHasCategory = !string.IsNullOrWhiteSpace(searchArgument.Category);

            if (searchArgsHasCategory)
            {
                query = query.Where(x => x.Category.Name.ToLower().Equals(searchArgument.Category.ToLower()));
            }

            if (searchArgsHasName)
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchArgument.Name.ToLower()));
            }

            return query.ToList();
        }
    }
}
