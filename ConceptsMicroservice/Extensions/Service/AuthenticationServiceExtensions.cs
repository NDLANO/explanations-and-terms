/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddConceptsAuthentication(this IServiceCollection services,
            Auth0Config config)
        {
            var scopes = new List<string>
            {
                config.Scope.ConceptAdmin,
                config.Scope.ConceptWrite,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = config.DomainUrl;
                options.Audience = config.Audience;
            });

            services.AddAuthorization(options =>
            {
                foreach (var scope in scopes)
                {
                    options.AddPolicy(scope,
                        policy => policy.Requirements.Add(new HasScopeRequirement(scope, config.DomainUrl)));
                }
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            return services;
        }
    }
}
