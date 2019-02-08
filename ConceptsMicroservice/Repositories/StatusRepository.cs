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
using ConceptsMicroservice.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ConceptsContext _context;
        public StatusRepository(ConceptsContext context)
        {
            _context = context;
        }

        public List<StatusDto> GetAll()
        {
            return _context.Status
                .Include(x => x.Language)
                .ToList();
        }

        public StatusDto GetById(int id)
        {
            return _context.Status
                .Include(x => x.Language)
                .FirstOrDefault(x => x.Id == id);
        }

        public StatusDto GetByName(string name)
        {
            return _context.Status
                .Include(x => x.Language)
                .FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }
    }
}