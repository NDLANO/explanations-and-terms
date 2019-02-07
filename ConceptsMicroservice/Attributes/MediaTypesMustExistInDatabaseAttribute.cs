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
    public class MediaTypesMustExistInDatabaseAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = validationContext.GetService(typeof(IConceptValidationService)) as IConceptValidationService;
            if (service == null)
                return new ValidationResult("Could not validate mediaTypes");

            var notExistingIds = service.MetaIdsDoesNotExistInDatabase(value as List<int>);
            if (notExistingIds.Any())
            {
                return new ValidationResult($"MediaTypes's [{string.Join(",", notExistingIds)}] is not a valid media type."); 
            }
            return ValidationResult.Success;
        }
    }
}
