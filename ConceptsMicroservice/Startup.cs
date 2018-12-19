/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Services.Validation;
using ConceptsMicroservice.Utilities;
using ConceptsMicroservice.Utilities.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConceptsMicroservice
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IConfigHelper _configHelper;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
            _configHelper = new ConfigHelper(_env, _config);
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            AddDependencies(services);
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ConceptsContext>(opt => opt.UseNpgsql(new DatabaseConfig(_configHelper).GetConnectionString()));
            services.AddSwagger();
            services.AddCors(
                options =>
                {
                    options.AddPolicy("AllowAll",
                        builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                }
                );
            services
                .AddMvc()
                    .AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureAuthentication(services);


            // To allow a uniform response in form of a Response if the action returns data, and ModelStateErrorResponse if the action returns an error.
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            SetOptions(services);
        }

        public void ConfigureAuthentication(IServiceCollection services)
        {
            var auth0Domain = $"https://{_configHelper.GetVariable(EnvironmentVariables.Auth0Domain)}/";
            var scopes = new List<string>
            {
                _configHelper.GetVariable(EnvironmentVariables.Auth0ScopeConceptWrite),
                _configHelper.GetVariable(EnvironmentVariables.Auth0ScopeConceptAdmin)
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = auth0Domain;
                options.Audience = _configHelper.GetVariable(EnvironmentVariables.Auth0Audience);
            });

            services.AddAuthorization(options =>
            {
                foreach (var scope in scopes)
                {
                    options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, auth0Domain)));
                }
            });

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseDefaultFiles();
            app.UseStaticFiles();

            ConfigureSwagger(app);

            app.UseAuthentication();

            app.UseCors("AllowAll");

            app.UseMvc();
        }

        /// <summary>
        /// Sets options according to the Options pattern
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.1
        /// </summary>
        /// <param name="services"></param>
        private void SetOptions(IServiceCollection services)
        {
            services.Configure<DatabaseConfig>(_config);
        }

        /// <summary>
        /// Handles the initialization and configurations for OpenAPI (swagger).
        /// Is configured to support multiple API versions.
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureSwagger(IApplicationBuilder app)
        {
            const string SWAGGER_BASE_URL = "/swagger";
            var v1_route = $"{SWAGGER_BASE_URL}/v1/swagger.json";

            app.UseSwaggerWithApiExplorer(config =>
            {
                config.GeneratorSettings.OperationProcessors.TryGet<ApiVersionProcessor>()
                    .IncludedVersions = new[] { "1.0" };
                config.SwaggerRoute = v1_route;

                config.GeneratorSettings.Title = "Concepts API";
                config.GeneratorSettings.Description = "API for NDLA's Concepts microservice";
            });

            app.UseSwaggerUi3(config =>
            {
                config.SwaggerRoutes.Add(new SwaggerUi3Route("v1.0", v1_route));
            });
        }
        public void AddDependencies(IServiceCollection services)
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
        }
    }
}
