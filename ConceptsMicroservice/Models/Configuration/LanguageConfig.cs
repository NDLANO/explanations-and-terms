/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;

namespace ConceptsMicroservice.Models.Configuration
{
    public class LanguageConfig
    {
        public string Default { get; set; }
        public List<string> Supported { get; set; }
    }
}
