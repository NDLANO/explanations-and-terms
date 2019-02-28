/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Models.Search
{
    public class MetaSearchQuery : BaseListQuery
    {
        /// <summary>
        /// The name of the metadata itself.
        /// </summary>
        [FromQuery]
        public string Name { get; set; } = "";
        /// <summary>
        /// The name of the metadata category.
        /// </summary>
        [FromQuery] public string Category { get; set; } = "";

        /// <summary>
        /// Checks whether the name or category has a value.
        /// </summary>
        /// <returns></returns>
        public bool HasNoQuery()
        {
            return string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Category);
        }

        public static MetaSearchQuery DefaultValues(string language)
        {
            return new MetaSearchQuery
            {
                Page = DefaultPage,
                PageSize = DefaultPageSize,
                Language = language
            };
        }
    }
}
