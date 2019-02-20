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

namespace ConceptsMicroservice.Repositories
{
    public class MediaTypeRepository : IMediaTypeRepository
    {
        private readonly Context.ConceptsContext _context;

        public MediaTypeRepository(Context.ConceptsContext context)
        {
            _context = context;
        }
        public List<MediaType> GetAll()
        {
            return _context.MediaTypes.ToList();
        }
    }
}