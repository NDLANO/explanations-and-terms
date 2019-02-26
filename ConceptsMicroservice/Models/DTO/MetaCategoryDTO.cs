/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Models.Domain;

namespace ConceptsMicroservice.Models.DTO
{
    public class MetaCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool CanHaveMultiple { get; set; }
        public bool IsRequired { get; set; }

        public Language Language { get; set; }
        public MetaCategoryTypeDTO CategoryType{ get; set; }
    }
}
