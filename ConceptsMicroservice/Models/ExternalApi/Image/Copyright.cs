/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;

namespace ConceptsMicroservice.Models.ExternalApi.Image
{
    public class Copyright
    {
        public List<TypeName> Processors { get; set; }
        public List<TypeName> Rightsholders { get; set; }
        public int AgreementId { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime ValidFrom { get; set; }
        public ImageLicense License {get; set; }
        public List<TypeName> Creators { get; set; }
        public string Origin { get; set; }
    }
}
