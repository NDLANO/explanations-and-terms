/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConceptsMicroservice.Models;

namespace ConceptsMicroservice.Attributes
{
    public class ConceptMustContainMetaAttribute :  ValidationAttribute
    {
        private readonly string[] _metaCategories;
        

        public ConceptMustContainMetaAttribute(params string[] metaCategories)
        {
            _metaCategories = metaCategories;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var metas = new List<MetaData>();
            if (value is MetaData meta)
                metas.Add(meta);
            else
            {
                metas = value as List<MetaData>;
            }

            if (metas == null || _metaCategories.Length > metas.Count)
            {
                return new ValidationResult(GetErrorMessage());
            }

            var categories = metas.Select(x => x.Category.Name).ToArray();
            var metaContainsAllCategories = _metaCategories.All(categories.Contains);

            return !metaContainsAllCategories ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return $"Concept requires {_metaCategories.Length} metas. The categories of these required meta is [{string.Join(", ", _metaCategories)}].";
        }
    }
}
