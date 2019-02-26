/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Domain;
using System.Linq;
using System;
namespace ConceptsMicroservice.Models.DTO
{
    public class StatusPagingDTO : PagingDTO
    {
        public List<Status> Results { get; set; }
        public StatusPagingDTO(List<Status> results, BaseListQuery query, string next) : base(query, next)
        {
            Results = results;

            try
            {
                TotalItems = results.FirstOrDefault().TotalItems;
                NumberOfPages = results.FirstOrDefault().NumberOfPages;
            }
            catch (NullReferenceException e) { }
        }
    }
}
