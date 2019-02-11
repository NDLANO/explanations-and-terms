/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

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
                services.AddSwaggerGen(config =>
                {
                    
                    config.SwaggerDoc(Version(description), new Info
                    {
                        Version = Version(description),
                        Title = "Explanations and terms API",
                        Description = "The Concept-API provides explanations of various concepts, and are meant used in articles and other contexts where a deeper understanding of the concept would be beneficial.\n\nThe API provides endpoints for searching after concepts and fetching explanations. Meta - data are also searched and attached to the returned explanations. Typical examples of meta - data are language, subject, and license. A concept may have different explanations for different metadata, e.g. an explanation of a concept can vary with the subject.",
                        TermsOfService = "https://om.ndla.no/tos/",
                        License = new License
                        {
                            Name = "GPL v3.0",
                            Url = "http://www.gnu.org/licenses/gpl-3.0.en.html"
                        },
                    });
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    config.IncludeXmlComments(xmlPath);
                });
            }

            return services;
        }

        public static IApplicationBuilder UseConceptSwaggerDocumentation(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    swaggerDoc.BasePath = "/concepts";
                });
            });
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{Version(description)}/swagger.json", Version(description));
                }
            });
            return app;
        }
    }
}
