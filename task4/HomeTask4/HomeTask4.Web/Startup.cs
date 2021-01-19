using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HomeTask4.Infrastructure.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using HomeTask4.Core.Entities;
namespace HomeTask4.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IValidator<Category>, CategoryValidator>();
            services.AddTransient<IValidator<Recipe>, RecipeValidator>();
            services.AddTransient<IValidator<RecipeStep>, RecipeStepValidator>();
            services.AddTransient<IValidator<Measure>, MeasureValidator>();
            services.AddTransient<IValidator<Ingredient>, IngredientValidator>();
            services.AddTransient<IValidator<IngredientDetail>, IngredientDetailValidator>();

            services.AddMvc().AddFluentValidation(fv => {
                fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });
            
            services.AddInfrastructure(Configuration.GetConnectionString("Default"));
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
