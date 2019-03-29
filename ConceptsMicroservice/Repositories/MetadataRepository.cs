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
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

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

        private IQueryable<MetaData> TableWithAllNestedObjects()
        {
            return _context.MetaData
                .Include(x => x.Language)
                .Include(x => x.Status)
                .ThenInclude(x => x.TypeGroup)
                .Include(x => x.Category)
                .ThenInclude(x => x.TypeGroup);
        }

        public List<MetaData> GetByRangeOfIds(List<int> ids)
        {
            return ids == null 
                ? new List<MetaData>() 
                : TableWithAllNestedObjects()
                    .Where(x => ids.Contains(x.Id)).ToList();
        }
        

        public List<MetaData> GetAll(BaseListQuery query)
        {
            var metadataWithQueriedLanguage = TableWithAllNestedObjects()
                .Where(a => a.Language.Abbreviation.Equals(query.Language))
                .ToList();
            var allMetaData = metadataWithQueriedLanguage;
            var totalItems = allMetaData.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / query.PageSize));
            if (query.Page > totalPages)
                query.Page = 1;

            var meta = allMetaData
                .OrderBy(x => x.Id)
                .Skip(query.PageSize * (query.Page - 1))
                .Take(query.PageSize)
                .ToList();
            meta.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
                x.Category = _context.Categories.Include(y => y.TypeGroup).FirstOrDefault(y => y.Id == x.CategoryId);
            });

            return meta;
        }

        public MetaData GetById(int id)
        {
            return TableWithAllNestedObjects()
                .FirstOrDefault(x => x.Id == id);
        }

        public List<MetaData> SearchForMetadata(MetaSearchQuery searchArgument)
        {
            var query = TableWithAllNestedObjects()
                .AsQueryable();

            if (searchArgument == null || searchArgument.HasNoQuery())
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

            var offset = searchArgument.PageSize * (searchArgument.Page - 1);

            var meta = query
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(searchArgument.PageSize)
                .ToList();
            meta.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });

            return meta;
        }

        public List<MetaData> GetLanguageVariationForThisList(List<MetaData> metas, int languageId)
        {
            List<MetaData> listOfMetaWithProperLanguage = new List<MetaData>();
            metas.ForEach(x =>
                {
                        listOfMetaWithProperLanguage.Add(item: _context.MetaData.FirstOrDefault(m =>
                            (m.LanguageId == languageId) && (m.LanguageVariation == GetById(x.Id).LanguageVariation)));
                }
            );
            return listOfMetaWithProperLanguage;
        }
    }
}
