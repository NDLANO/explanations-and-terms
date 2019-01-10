using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddConceptsSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Security = new List<SwaggerSecurityRequirement>();
                    document.Info.Version = "v1";
                    document.Info.Title = "Explanations and terms API";
                    document.Info.Description = "Services for accessing explanations from NDLA.";
                    document.Info.TermsOfService = "https://ndla.no/";
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "GPL v3.0",
                        Url = "http://www.gnu.org/licenses/gpl-3.0.en.html"
                    };

                    document.SecurityDefinitions.Add("Bearer", new NSwag.SwaggerSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        Type = NSwag.SwaggerSecuritySchemeType.ApiKey,
                        In = NSwag.SwaggerSecurityApiKeyLocation.Header
                    });
                    document.Security.Add(new NSwag.SwaggerSecurityRequirement { { "Bearer", new string[] { } }, });
                };
            });

            return services;
        }

        public static IApplicationBuilder UseConceptSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    AppName = "Explanations and terms NDLA",
                    ClientId = "",
                    ClientSecret = "",

                };

            });
            app.UseSwagger();

            return app;
        }
    }
}
