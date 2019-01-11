/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
