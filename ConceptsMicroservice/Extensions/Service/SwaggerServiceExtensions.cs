/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ConceptsMicroservice.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.WebApi;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class SwaggerServiceExtensions
    {
        
        public static IServiceCollection AddConceptsSwaggerDocumentation(this IServiceCollection services,
            IConfigHelper configHelper)
        {

            return services;
        }

        private static string SwaggerJsonPath(string version)
        {
            return $"swagger/v{version}/swagger.json";
        }

        public static IApplicationBuilder UseConceptSwaggerDocumentation(this IApplicationBuilder app
            , IConfigHelper configHelper, IApiVersionDescriptionProvider provider)
        {
            
            foreach (var description in provider.ApiVersionDescriptions)
            {
                app.UseSwaggerUi3WithApiExplorer(config =>
                {
                    config.GeneratorSettings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { description.ApiVersion.ToString() };
                    config.SwaggerRoute = SwaggerJsonPath(description.ApiVersion.ToString());
                   
                });
            }
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, config =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    var version = description.ApiVersion.ToString();
                    config.SwaggerRoutes.Add(new SwaggerUi3Route($"v{version}", $"/{SwaggerJsonPath(version)}"));
                }
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
                config.GeneratorSettings.Title = "Explanations and terms API";
                config.GeneratorSettings.Description = "Documentation for the Explanation and terms API from NDLA.";
            });


            return app;
        }
    }
}
