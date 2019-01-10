using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Services.Validation;
using ConceptsMicroservice.Utilities;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class DependenciesServiceExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<IConceptService, ConceptService>();
            services.AddScoped<IConceptRepository, ConceptRepository>();

            services.AddScoped<IMetadataService, MetadataService>();
            services.AddScoped<IMetadataRepository, MetadataRepository>();

            services.AddScoped<IStatusRepository, StatusRepository>();
            services.AddScoped<IStatusService, StatusService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();


            services.AddScoped<IDatabaseConfig, DatabaseConfig>();
            services.AddScoped<ITokenHelper, TokenHelper>();
            services.AddScoped<IConfigHelper, ConfigHelper>();

            services.AddScoped<IConceptValidationService, ConceptValidationService>();

            return services;
        }
    }
}
