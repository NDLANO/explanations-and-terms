/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;

namespace ConceptsMicroservice.Models.ExternalApi.Image
{
    public class SingleImageQuery : Base
    {
        [Required]
        public string ImageId { get; set; }
        public string Language { get; set; }
    }
}
