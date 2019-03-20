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
using ConceptsMicroservice.Services.Validation;

namespace ConceptsMicroservice.Attributes
{
    public class MustContainOneOfEachCategoryAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.GetService(typeof(IConceptValidationService)) is IConceptValidationService service))
                return new ValidationResult("Could not validate metaid's");

            var missingCategories = service.GetMissingRequiredCategories(value as List<int>);
            if (missingCategories.Any())
                return new ValidationResult($"Missing required meta(s) of category [{string.Join(", ", missingCategories)}]");

            return ValidationResult.Success;
        }
    }
}
