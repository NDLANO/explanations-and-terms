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
    public class MetaIdsMustExistInDatabaseAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = (IConceptValidationService)validationContext
                .GetService(typeof(IConceptValidationService));
            if (service == null)
                return new ValidationResult("Could not validate metaIds");

            var notExistingIds = service.MetaIdsDoesNotExistInDatabase((IEnumerable<int>)value);
            if (notExistingIds.Any())
            {
                return new ValidationResult($"Metaid's({string.Join(",", notExistingIds)}) is not a valid metas."); 
            }
            return ValidationResult.Success;
        }
    }
}
