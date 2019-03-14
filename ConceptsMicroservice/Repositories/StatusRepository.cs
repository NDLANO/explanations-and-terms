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
    public class StatusRepository : IStatusRepository
    {
        private readonly Context.ConceptsContext _context;
        public StatusRepository(Context.ConceptsContext context)
        {
            _context = context;
        }

        public List<Status> GetAll(BaseListQuery query)
        {
            var allStatus = _context.Status
                .Include(x => x.Language)
                .Include(x => x.TypeGroup)
                .Where(x => x.Language.Abbreviation.Equals(query.Language));

            var totalItems = allStatus.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling(totalItems * 1.0 / query.PageSize));
            if (query.Page > totalPages)
                query.Page = 1;

            var status = allStatus
                .OrderBy(x => x.Id)
                .Skip(query.PageSize * (query.Page - 1))
                .Take(query.PageSize)
                .ToList();
            status.ForEach(x =>
            {
                x.TotalItems = totalItems;
                x.NumberOfPages = totalPages;
            });

            return status;
        }

        public Status GetById(int id)
        {
            return _context.Status
                .Include(x => x.Language)
                .FirstOrDefault(x => x.Id == id);
        }

        public Status GetByName(string name)
        {
            return _context.Status
                .Include(x => x.Language)
                .FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }
    }
}