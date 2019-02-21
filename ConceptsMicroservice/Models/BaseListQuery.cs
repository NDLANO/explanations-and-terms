/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Models
{
    public class BaseListQuery : BaseQuery
    {
        /// <summary>
        /// The page number of the search hits to display
        /// </summary>
        [FromQuery] public int Page { get; set; }
        /// <summary>
        /// The number of search hits to display for each page
        /// </summary>
        [FromQuery] public int PageSize { get; set; }

        public static BaseListQuery DefaultValues(string language)
        {
            return new BaseListQuery
            {
                Page = 1,
                PageSize = 10,
                Language = language
            };
        }
    }
}
