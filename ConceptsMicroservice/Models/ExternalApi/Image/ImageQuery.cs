/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.Models.ExternalApi.Image
{
    public class ImageQuery : Base
    {
        public string Query { get; set; }
        public int MinimumSize { get; set; }
        public string Language { get; set; }
        public string License { get; set; }
        public bool IncludeCopyrighted { get; set; }
        public string Sort { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchContext { get; set; }
    }
}
