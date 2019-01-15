/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Models.ExternalApi
{
    public class Response
    {
        public int TotalCount { get; set; }
        public string Language { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

    }
}
