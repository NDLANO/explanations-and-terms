/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Utilities;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConceptsMicroservice.Extensions.Service
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddConceptsAuthentication(this IServiceCollection services,
            IConfiguration config)
        {
            var auth0Config = new Auth0Config();
            config.GetSection(ConfigSections.Auth0).Bind(auth0Config);

            var scopes = new List<string>
            {
                auth0Config.Scope.ConceptAdmin,
                auth0Config.Scope.ConceptWrite,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = auth0Config.DomainUrl;
                options.Audience = auth0Config.Audience;
            });

            services.AddAuthorization(options =>
            {
                foreach (var scope in scopes)
                {
                    options.AddPolicy(scope,
                        policy => policy.Requirements.Add(new HasScopeRequirement(scope, auth0Config.DomainUrl)));
                }
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            return services;
        }
    }
}
