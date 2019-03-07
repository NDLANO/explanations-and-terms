/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
 
using System.Linq;
using ConceptsMicroservice.Models.Domain;

namespace ConceptsMicroservice.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly Context.ConceptsContext _context;
        public LanguageRepository(Context.ConceptsContext context)
        {
            _context = context;
        }

        public Language GetByAbbreviation(string abbreviation)
        {
            return _context.Languages.FirstOrDefault(x => x.Abbreviation.Equals(abbreviation));
        }
    }
}