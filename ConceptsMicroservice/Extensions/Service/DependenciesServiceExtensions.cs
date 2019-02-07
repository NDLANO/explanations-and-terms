/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Services.Validation;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class DependenciesServiceExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
            services.AddHelpers();
            services.AddSingletons();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IConceptRepository, ConceptRepository>();
            services.AddScoped<IMetadataRepository, MetadataRepository>();
            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMediaTypeRepository, MediaTypeRepository>();
            services.AddScoped<IConceptMediaRepository, ConceptMediaRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IConceptService, ConceptService>();
            services.AddScoped<IMetadataService, MetadataService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMediaTypeService, MediaTypeService>();

            services.AddScoped<IConceptValidationService, ConceptValidationService>();
            return services;
        }

        private static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddScoped<ITokenHelper, TokenHelper>();
            return services;
        }

        private static IServiceCollection AddSingletons(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
