/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Services.Validation;

namespace ConceptsMicroservice.Attributes
{
    public class StatusIdExistsInDatabaseAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = validationContext.GetService(typeof(IConceptValidationService)) as IConceptValidationService;
            if (service == null)
                return new ValidationResult("Could not validate status id");

            var id = (int)value;
            if (service.StatusIdIsValidId(id))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("StatusId is not a valid id.");
        }
    }
}
