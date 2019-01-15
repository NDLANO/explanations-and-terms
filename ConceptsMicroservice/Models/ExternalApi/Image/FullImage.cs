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
    public class FullImage
    {
        public Copyright Copyright { get; set; }
        public int Size{ get; set; }
        public ImageTag Tags { get; set; }
        public ImageAltText AltText { get; set; }
        public string MetaUrl { get; set; }
        public string Id { get; set; }
        public string ContentType { get; set; }
        public ImageCaption Caption { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public ImageTitle Title { get; set; }
        public string ImageUrl { get; set; }
    }
}
