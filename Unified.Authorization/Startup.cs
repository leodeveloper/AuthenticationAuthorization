using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using Unified.Authorization.Models;
using Unified.Authorization.Repository;

namespace Unified.Authorization
{

    /// <summary>
    /// Very important for Role Authentication:
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0
    /// </summary>
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }                

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerDocument();

            services.AddControllers();   
            services.AddOptions();

            services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(SharedResources).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResources", assemblyName.Name);
                };
            }); 
                    

            services.Configure<UserManagerConStrings>(Configuration.GetSection("UserManagerConStrings"));
            services.Configure<EntitiesApi>(Configuration.GetSection("EntitiesApi"));

            //var conStrings = Configuration.GetSection("UserManagerConStrings");
            //string conStr = (string)conStrings.GetValue<String>("DBConnectionString");

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conStr));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
                (Configuration.GetSection("UserManagerConStrings").GetValue<string>("DBConnectionString")));
                //(Configuration.GetConnectionString("DBConnectionString"))); // this does not work because it reads from ConnectionStrings node
                            

            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IHraApplicationRepository, HraApplicationRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IClaimTypeRepository, ClaimTypeRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleEditRepository, RoleEditRepository>();                        
            services.AddScoped<IRoleClaimsEditRepository, RoleClaimsEditRepository>();            
            services.AddScoped<IUsersEditRepository, UsersEditRepository>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-AE") };
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");                
                options.SupportedUICultures = supportedCultures;
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"C:\\Logs\\LogAuthorization.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            
            app.UseSwaggerUi3();
            app.UseOpenApi();
            

        }
    }
}
