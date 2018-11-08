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

            var jwtKey = Environment.GetEnvironmentVariable("PETGAME_JWT_KEY");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_token";
                });

            // cookie authentication scheme
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie();
            // jwt authentication scheme
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            //        {
            //            // validate that the server created the token
            //            ValidateIssuer = false,
            //            // ensure that the recipient of the token is authorized to recieve it
            //            ValidateAudience = false,
            //            // check that thetoken is not expired and the signing key of issuer is valid
            //            ValidateLifetime = true,
            //            // verify that the key used to sign the incoming token is part of a list of 
            //            // trusted keys
            //            ValidateIssuerSigningKey = true,
            //            // todo add ValidIssuers
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            //        };
            //    }).AddCookie(options =>
            //    {
            //        options.Cookie.Expiration = TimeSpan.FromDays(7);
            //        options.SlidingExpiration = true;
            //        options.Cookie.HttpOnly = true;
            //        options.Cookie.Name = "auth_token";
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
