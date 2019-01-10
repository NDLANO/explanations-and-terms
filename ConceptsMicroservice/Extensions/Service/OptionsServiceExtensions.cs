using ConceptsMicroservice.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class OptionsServiceExtensions
    {
        /// <summary>
        /// Sets options according to the Options pattern
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.1
        /// </summary>
        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<DatabaseConfig>(config);

            return services;
        }
    }
}
