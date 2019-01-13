/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NSwag;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class SwaggerServiceExtensions
    {

        public static string Version(ApiVersionDescription apiVersion)
        {
            return $"{apiVersion.GroupName}".ToString();
        }
        public static IServiceCollection AddConceptsSwaggerDocumentation(this IServiceCollection services)
        {

            var provider = services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                services.AddSwaggerDocument(config =>
                {
                    config.DocumentName = Version(description);

                    config.ApiGroupNames = new[] { Version(description) };
                    config.PostProcess = document =>
                    {
                        document.Security = new List<SwaggerSecurityRequirement>();
                        document.Info.Title = "Explanations and terms API";
                        document.Info.Description = "Documentation for the Explanation and terms API from NDLA.";
                        document.Info.TermsOfService = "https://ndla.no/";
                        document.Info.License = new NSwag.SwaggerLicense
                        {
                            Name = "GPL v3.0",
                            Url = "http://www.gnu.org/licenses/gpl-3.0.en.html"
                        };
                        document.Schemes = new[] { SwaggerSchema.Https };
                    };
                });
            }
            return services;
        }

        public static IApplicationBuilder UseConceptSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUi3();
            return app;
        }
    }
}
