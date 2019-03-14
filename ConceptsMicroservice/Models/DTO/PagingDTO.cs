/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;

namespace ConceptsMicroservice.Models.DTO
{
    public class PagingDTO<T>
    {
        public int TotalItems { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int NumberOfPages { get; set; } = 1;
        public string Next { get; set; }

        public List<T> Results { get; set; }
        public PagingDTO() { }

        public PagingDTO(List<T> results, BaseListQuery query, string next, int pages = 1, int totalItems = 0)
        {
            PageSize = query.PageSize;
            Page = query.Page;
            TotalItems = totalItems;
            NumberOfPages = pages;
            Results = results;

            if (Page < NumberOfPages)
            {
                query.Page += 1;
                Next = next;
            }
            
        }
    }
}
