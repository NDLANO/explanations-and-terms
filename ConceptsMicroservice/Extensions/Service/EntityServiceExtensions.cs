/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class EntityServiceExtensions
    {
        public static IServiceCollection AddEntity(this IServiceCollection services, IConfiguration config)
        {
            var databaseConfig = new DatabaseConfig();
            config.GetSection(ConfigSections.Database).Bind(databaseConfig);

            services.AddEntityFrameworkNpgsql();
            services.AddDbContext<Context.ConceptsContext>(opt => opt.UseNpgsql(databaseConfig.ConnectionString));

            return services;
        }
    }
}
