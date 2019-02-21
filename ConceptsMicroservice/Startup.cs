/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConceptsMicroservice.Extensions.Service;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
            services.AddAutoMapper();
            services.AddEntity(_config);
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCorsForConcepts();
            services.AddConceptsAuthentication(_config);

            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddConceptApiVersioning();


            // To allow a uniform response in form of a Response if the action returns data, and ModelStateErrorResponse if the action returns an error.
            services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

            // For IURlHelper
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

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
