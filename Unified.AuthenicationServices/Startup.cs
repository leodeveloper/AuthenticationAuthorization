using ApplicationPOCO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Unified.Model;
using System;
using Unified.Commonlibs.Service;
using Unified.Commonlibs.Utility;
using Unified.CommonServices.Respository;

namespace Unified.AuthenicationServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment HostingEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            string key = Configuration["Localsettings:jwtsecret"];

            services.AddOptions();
            services.Configure<RediscacheSettings>(Configuration.GetSection("RediscacheSettings"));
            services.Configure<Appsettings>(Configuration.GetSection("Appsettings"));
            services.Configure<Localsettings>(Configuration.GetSection("Localsettings"));

            services.AddControllers();
            // For Entity Framework
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnStr")));

            // For Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>   opt.TokenLifespan = TimeSpan.FromHours(2));
            if(HostingEnvironment.IsDevelopment())
                services.AddJwtAuthentication(HostingEnvironment, Configuration, false );
            else
                services.AddJwtAuthentication(HostingEnvironment, Configuration, true);


            services.AddDataProtection();
            services.AddSingleton<IJwtAuthentication>(new JwtAuthentication(key));
            services.AddSingleton<ICommunicationService, CommunicationService>();
            services.AddSingleton<IRedisRespository, RedisRespository>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
