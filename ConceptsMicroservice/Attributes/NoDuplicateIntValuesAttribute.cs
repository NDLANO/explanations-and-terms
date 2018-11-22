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
using ConceptsMicroservice.Utilities;

namespace ConceptsMicroservice.Attributes
{
    public class NoDuplicateIntValuesAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var duplicates = Helpers.GetDuplicates(value as IEnumerable<int>).ToArray();
            if (duplicates.Any())
            {
                return new ValidationResult("Duplicate values is not allowed"); 
            }

            return ValidationResult.Success;
        }
    }
}
