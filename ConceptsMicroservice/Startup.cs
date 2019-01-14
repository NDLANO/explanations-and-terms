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
using ConceptsMicroservice.Utilities;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
            services.AddDependencies();
            
            services.AddEntity(_configHelper);

            services.AddCorsForConcepts();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddConceptApiVersioning();

            services.AddConceptsAuthentication(_configHelper);

            // To allow a uniform response in form of a Response if the action returns data, and ModelStateErrorResponse if the action returns an error.
            services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

            services.AddOptions(_config);

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

            app.UseMvc();

            app.UseConceptSwaggerDocumentation(provider);

            app.UseAuthentication();

            app.UseCors(CorsServiceExtensions.AllowAll);

        }
    }
}
