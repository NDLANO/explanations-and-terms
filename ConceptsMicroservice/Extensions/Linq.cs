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
using ConceptsMicroservice.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<MetaData> IncludeAll(this IQueryable<MetaData> query)
        {
            return query == null ? query : query.Include(x => x.Category).Include(x => x.Status);
        }

        public static List<Concept> ToListWithMetadata(this IQueryable<Concept> query, IQueryable<MetaData> metadata)
        {
            if (query == null)
                return null;

        var concepts = query.ToList();
        foreach (var concept in concepts)
        {
            concept.Meta = metadata.IncludeAll().Where(x => concept.MetaIds.Contains(x.Id)).ToList();
        }

        return concepts;
    }

        public static Concept FirstOrDefaultWithMetadata(this IEnumerable<Concept> query, IQueryable<MetaData> metadata)
        {
            var concept = query?.FirstOrDefault();
            if (concept == null)
                return null;

            concept.Meta = metadata.IncludeAll().Where(x => concept.MetaIds.Contains(x.Id)).ToList();

            return concept;
        }

        public static Concept FirstOrDefaultWithMetadata(this IEnumerable<Concept> query, Func<Concept, bool> func, IQueryable<MetaData> metadata)
        {
            var concept = query?.FirstOrDefault(func);
            if (concept == null)
                return null;

            concept.Meta = metadata.IncludeAll().Where(x => concept.MetaIds.Contains(x.Id)).ToList();

            return concept;
        }

    }
}
