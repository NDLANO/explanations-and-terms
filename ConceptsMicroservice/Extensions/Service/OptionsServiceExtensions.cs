/** Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using ConceptsMicroservice.Models.Configuration;
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
            services.Configure<Auth0Config>(config.GetSection(ConfigSections.Auth0));
            services.Configure<DatabaseConfig>(config.GetSection(ConfigSections.Database));
            services.Configure<LanguageConfig>(config.GetSection(ConfigSections.Language));

            return services;
        }
    }
}
