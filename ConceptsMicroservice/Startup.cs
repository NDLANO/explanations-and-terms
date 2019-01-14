/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConceptsMicroservice.Extensions.Service;
using ConceptsMicroservice.Models.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConceptsMicroservice
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(_config);
            services.AddDependencies();

            var auth0Config = new Auth0Config();
            _config.GetSection("Auth0").Bind(auth0Config);


            var databaseConfig = new DatabaseConfig();
            _config.GetSection("Database").Bind(databaseConfig);

            services.AddEntity(databaseConfig);

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCorsForConcepts();
            services.AddConceptsAuthentication(auth0Config);

            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddConceptApiVersioning();


            // To allow a uniform response in form of a Response if the action returns data, and ModelStateErrorResponse if the action returns an error.
            services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);
            
            services.AddConceptsSwaggerDocumentation();
        }
        
        
        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseCors(CorsServiceExtensions.AllowAll);

            app.UseConceptSwaggerDocumentation(provider);

            app.UseAuthentication();

            app.UseMvc();

        }
    }
}
