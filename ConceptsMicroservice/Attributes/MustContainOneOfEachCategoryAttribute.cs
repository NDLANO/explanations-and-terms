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
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Services.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Attributes
{
    public class MustContainOneOfEachCategoryAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.GetService(typeof(IOptions<LanguageConfig>)) is IOptions<LanguageConfig> languageService))
                return new ValidationResult("Could not validate language");

            var languageConfig = languageService.Value;

            if (!(validationContext.GetService(typeof(IHttpContextAccessor)) is IHttpContextAccessor httpContext))
                return new ValidationResult("Could not validate language with request params");

            var language = languageConfig.Default;

            if (!string.IsNullOrEmpty(httpContext.HttpContext.Request.Query["language"]) 
                && languageConfig.Supported.Contains(httpContext.HttpContext.Request.Query["language"]))
            {
                language = httpContext.HttpContext.Request.Query["language"];
            }

            if (!(validationContext.GetService(typeof(IConceptValidationService)) is IConceptValidationService service))
                return new ValidationResult("Could not validate metaid's");

            var missingCategories = service.GetMissingRequiredCategories(value as List<int>, language);
            if (missingCategories.Any())
                return new ValidationResult($"Missing required meta(s) of category [{string.Join(", ", missingCategories)}]");

            return ValidationResult.Success;
        }
    }
}
