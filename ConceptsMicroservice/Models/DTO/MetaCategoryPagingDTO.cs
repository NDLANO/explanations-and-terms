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

namespace ConceptsMicroservice.Models.DTO
{
    public class MetaCategoryPagingDTO : PagingDTO
    {
        public List<MetaCategory> Results { get; set; }

        public MetaCategoryPagingDTO(List<MetaCategory> results, BaseListQuery query, string next) : base(query, next)
        {
            Results = results;

            try
            {
                TotalItems = results.FirstOrDefault().TotalItems;
                NumberOfPages = results.FirstOrDefault().NumberOfPages;
            }
            catch(NullReferenceException e) { }
        }
    }
}
