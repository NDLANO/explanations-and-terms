/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Utilities;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddConceptsAuthentication(this IServiceCollection services,
            IConfigHelper configHelper)
        {
            var auth0Domain = $"https://{configHelper.GetVariable(EnvironmentVariables.Auth0Domain)}/";
            var scopes = new List<string>
            {
                configHelper.GetVariable(EnvironmentVariables.Auth0ScopeConceptWrite),
                configHelper.GetVariable(EnvironmentVariables.Auth0ScopeConceptAdmin)
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = auth0Domain;
                options.Audience = configHelper.GetVariable(EnvironmentVariables.Auth0Audience);
            });

            services.AddAuthorization(options =>
            {
                foreach (var scope in scopes)
                {
                    options.AddPolicy(scope,
                        policy => policy.Requirements.Add(new HasScopeRequirement(scope, auth0Domain)));
                }
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            return services;
        }
    }
}
