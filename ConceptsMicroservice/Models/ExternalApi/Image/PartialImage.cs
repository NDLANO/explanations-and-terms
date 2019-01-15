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
    public class PartialImage
    {
        public ImageAltText AltText { get; set; }
        public string License { get; set; }
        public string MetaUrl { get; set; }
        public string Id { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public ImageTitle Title { get; set; }
        public List<string> Contributors { get; set; }
        public string PreviewUrl { get; set; }
    }
}
