/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddConceptsSwaggerDocumentation(this IServiceCollection services,
            IConfigHelper configHelper)
        {

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Security = new List<SwaggerSecurityRequirement>();
                    document.Info.Version = "v1";
                    document.Info.Title = "Explanations and terms API";
                    document.Info.Description = "Documentation for the Explanation and terms API from NDLA.";
                    document.Info.TermsOfService = "https://ndla.no/";
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "GPL v3.0",
                        Url = "http://www.gnu.org/licenses/gpl-3.0.en.html"
                    };
                    document.Schemes = new[] {SwaggerSchema.Https};
                    /*

                    const string OauthSecurity = "oauth2";
                    var conceptWrite = "writeScope;
                    var oauthDomain = "domain";
                    document.SecurityDefinitions.Add(OauthSecurity, new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Flow = SwaggerOAuth2Flow.Implicit,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                Scopes = new Dictionary<string, string> {{"label", "value"} },
                                AuthorizationUrl = $"https://{oauthDomain}"
                            }
                        }
                    });
                    document.Security.Add(new NSwag.SwaggerSecurityRequirement { { OauthSecurity, new string[] { } }, });
                    */
                };
            });

            return services;
        }

        public static IApplicationBuilder UseConceptSwaggerDocumentation(this IApplicationBuilder app
            , IConfigHelper configHelper)
        {
            app.UseSwaggerUi3(options =>
            {
                /*options.OAuth2Client = new OAuth2ClientSettings
                {
                    AppName = "Explanations and terms NDLA",
                };*/

            });
            app.UseSwagger();

            return app;
        }
    }
}
