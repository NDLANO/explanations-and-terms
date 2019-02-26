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
            var languageConfig =
                (validationContext.GetService(typeof(IOptions<LanguageConfig>)) as IOptions<LanguageConfig>).Value;
            var httpContext = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var language = languageConfig.Default;

            if (httpContext != null 
                && !string.IsNullOrEmpty(httpContext.HttpContext.Request.Query["language"]) 
                && languageConfig.Supported.Contains(httpContext.HttpContext.Request.Query["language"]))
            {
                language = httpContext.HttpContext.Request.Query["language"];
            }

            var service = validationContext.GetService(typeof(IConceptValidationService)) as IConceptValidationService;
            if (service == null)
                return new ValidationResult("Could not validate metaid's");

            var missingCategories = service.GetMissingRequiredCategories(value as List<int>, language);
            if (missingCategories.Any())
                return new ValidationResult($"Missing required meta(s) of category [{string.Join(", ", missingCategories)}]");

            return ValidationResult.Success;
        }
    }
}
