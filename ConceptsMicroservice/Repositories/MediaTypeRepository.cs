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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Repositories
{
    public class MediaTypeRepository : IMediaTypeRepository
    {
        private readonly Context.ConceptsContext _context;
        private readonly LanguageConfig _languageConfig;
        public MediaTypeRepository(Context.ConceptsContext context, IOptions<LanguageConfig> languageConfig)
        {
            _context = context;
            _languageConfig = languageConfig.Value;
        }

        public List<MediaType> GetAll(BaseListQuery query)
        {
            var mediaTypes = _context.MediaTypes
                .Include(x => x.Language)
                .Include(x => x.TypeGroup)
                .Where(x => x.Language.Abbreviation.Equals(query.Language));

            var totalItems = mediaTypes.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / query.PageSize));
            if (query.Page > totalPages)
                query.Page = 1;

            var data = mediaTypes
                .Skip(query.PageSize * (query.Page - 1))
                .Take(query.PageSize)
                .ToList();
            data.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });

            return data;
        }
    }
}