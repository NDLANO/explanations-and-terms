/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

namespace ConceptsMicroservice.Models.ExternalApi.Image
{
    public class Error
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public string OccuredAt { get; set; }
    }
}
