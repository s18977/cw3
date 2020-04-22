using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Handlers;
using cw3.Middlewares;
using cw3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace cw3
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
            services.AddSingleton<IStudentsDbService, EnrollmentDbService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "Gakko",
                        ValidAudience = "Students",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                    };
                });

            //services.AddAuthentication("AuthenticationBasic").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("AuthenticationBasic", null);
            services.AddControllers()
                    .AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDbService dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<LoggingMiddleware>();

            //app.Use(async (context, next) =>
            //{
            //    if(!context.Request.Headers.ContainsKey("Index"))
            //    {
            //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        await context.Response.WriteAsync("Nie podano indeksu w nag�owku");
            //        return;
            //    }

            //    var index = context.Request.Headers["Index"].ToString();

            //    if (!dbService.CheckIndex(index))
            //    {
            //        context.Response.StatusCode = StatusCodes.Status404NotFound;
            //        await context.Response.WriteAsync("Student o podanym indeksie nie wystepuje w bazie"); 
            //        return;
            //    }

            //    await next();
            //});



            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
