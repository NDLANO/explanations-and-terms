/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ConceptsMicroservice.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        private readonly Context.ConceptsContext _context;
        private readonly LanguageConfig _languageConfig;
        public MetadataRepository(Context.ConceptsContext context,  IOptions<LanguageConfig> languageConfig)
        {
            _context = context;
            _languageConfig = languageConfig.Value;
        }

        public List<MetaData> GetByRangeOfIds(List<int> ids)
        {
            return ids == null 
                ? new List<MetaData>() 
                : _context.MetaData
                    .Include(x => x.Language)
                    .Where(x => ids.Contains(x.Id)).ToList();
        }
        

        public List<MetaData> GetAll(BaseListQuery query)
        {
            var allMetaData = _context.MetaData
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.Status)
                .Where(x => x.Language.Abbreviation.Equals(query.Language));

            var totalItems = allMetaData.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / query.PageSize));
            if (query.Page > totalPages)
                query.Page = 1;

            var meta = allMetaData
                .Skip(query.PageSize * (query.Page - 1))
                .Take(query.PageSize)
                .ToList();
            meta.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });

            return meta;
        }

        public MetaData GetById(int id)
        {
            return _context.MetaData
                .Include(x => x.Language)
                .Include(x => x.Category)
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<MetaData> SearchForMetadata(MetaSearchQuery searchArgument)
        {
            var query = _context.MetaData
                .Include(x => x.Status)
                .Include(x => x.Category)
                .Include(x => x.Language)
                .AsQueryable();

            if (searchArgument == null)
                return GetAll(BaseListQuery.DefaultValues(_languageConfig.Default));

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

            var totalItems = query.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / searchArgument.PageSize));

            if (searchArgument.Page > totalPages)
                searchArgument.Page = 1;

            var meta = query
                .Skip(searchArgument.PageSize * (searchArgument.Page - 1))
                .Take(searchArgument.PageSize)
                .ToList();
            meta.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });

            return meta;
        }
    }
}
