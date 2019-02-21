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
    public class ConceptResultDTO
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int NumberOfPages { get; set; }
        public string Next { get; set; }
        public List<ConceptDto> Concepts { get; set; }
    }
}
