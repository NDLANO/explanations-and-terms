﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using ConceptsMicroservice.Models.Configuration;

namespace ConceptsMicroservice.Models
{
    public class BaseListQuery : BaseQuery
    {
        public static readonly int DefaultPageSize = 10;
        public static readonly int MaxPageSize = 100;
        public static readonly int DefaultPage = 1;

        /// <summary>
        /// The page number of the search hits to display
        /// </summary>
        [Range(1, Int32.MaxValue, ErrorMessage = "Minimum page is 1")]
        [FromQuery] public int Page { get; set; }
        /// <summary>
        /// The number of search hits to display for each page
        /// </summary>
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        [FromQuery] public int PageSize { get; set; }

        public void SetDefaultValuesIfNotInitilized(LanguageConfig config)
        {
            if (Page < 1)
                Page = 1;
            if (!config.Supported.Contains(Language))
                Language = config.Default;
            if (PageSize < 1 || PageSize > MaxPageSize)
                PageSize = DefaultPageSize;
        }

        public static BaseListQuery DefaultValues(string language)
        {
            return new BaseListQuery
            {
                Page = DefaultPage,
                PageSize = DefaultPageSize,
                Language = language
            };
        }
    }
}