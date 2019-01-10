using ConceptsMicroservice.Context;
using ConceptsMicroservice.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class EntityServiceExtensions
    {
        public static IServiceCollection AddEntity(this IServiceCollection services, IConfigHelper configHelper)
        {
            var connectionString = new DatabaseConfig(configHelper).GetConnectionString();
            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<ConceptsContext>(opt => opt.UseNpgsql(connectionString));

            return services;
        }
    }
}
