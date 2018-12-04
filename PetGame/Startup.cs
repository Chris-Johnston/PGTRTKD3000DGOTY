using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetGame.Core;
using Microsoft.AspNetCore.Diagnostics;

namespace PetGame
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // connection string must be set in the environment variables to connect to a MSSQL db instance
            var connectionString = Environment.GetEnvironmentVariable("PETGAME_DB_CONNECTION_STRING");
            services.AddSingleton<SqlManager>(new SqlManager(connectionString));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_token";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // in development we should throw a more comprehensive exception page
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // but in prod, only show minimal details
                // in this case, we are probably fine just saying what type of Exception
                // was thrown
                app.UseExceptionHandler(options =>
                {
                    options.Run(async context =>
                    {
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = errorFeature.Error;
                        // return plaintext
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(
                            $"Internal Server Error: {exception.GetType().ToString()}").ConfigureAwait(false);
                    });
                });
            }

            // run and forget without await
            _ = app.UseStatusCodePages(async context =>
            {
                // return plaintext
                context.HttpContext.Response.ContentType = "text/plain";
                // just return "Status: 404" or whatever for all errors
                // we *could* do something fancier here, but later
                await context.HttpContext.Response.WriteAsync(
                    $"Status: {context.HttpContext.Response.StatusCode}").ConfigureAwait(false);
            });

            // for files under wwwroot
            app.UseStaticFiles();

            // use JWT authentication
            app.UseAuthentication();
            app.UseFileServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id}");
            });
        }
    }
}
