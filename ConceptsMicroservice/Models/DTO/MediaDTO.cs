/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;

namespace ConceptsMicroservice.Models.Domain
{
    
    public class MediaDTO
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Source { get; set; }
        public MediaTypeDTO MediaType { get; set; }
    }
}
