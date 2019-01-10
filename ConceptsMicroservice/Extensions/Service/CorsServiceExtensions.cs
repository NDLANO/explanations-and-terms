﻿using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class CorsServiceExtensions
    {
        public static readonly string AllowAll = "AllowAll";
        /// <summary>
        /// Sets options according to the Options pattern
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.1
        /// </summary>
        public static IServiceCollection AddCorsForConcepts(this IServiceCollection services)
        {
            services.AddCors(
                options =>
                {
                    options.AddPolicy(AllowAll,
                        builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                }
            );

            return services;
        }
    }
}
