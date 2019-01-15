/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;

namespace ConceptsMicroservice.Models.ExternalApi.Image
{
    public class SingleImageResponse : Response
    {
        public FullImage Result { get; set; }
    }
}
