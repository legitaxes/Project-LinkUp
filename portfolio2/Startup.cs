using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace portfolio2
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
            // Adds the authentication services
            services.AddAuthentication(options =>
            {
                // Use a cookie to locally sign-in the user
                options.DefaultScheme =
                CookieAuthenticationDefaults.AuthenticationScheme;
                // Use OpenID Connect protocol to login
                options.DefaultChallengeScheme = "oidc";
            })            .AddCookie()
             //Configure the handler that perform the OpenID Connect protocol
             .AddOpenIdConnect("oidc", options =>
             {
                 //The server to process the authentication
                 options.Authority = "https://ictonejourney.com";
                 //To identify the client
                 options.ClientId = "ojweb-practical";
                 options.ClientSecret =
                 "SG54frqHvy6K6wk2+C7bOngKp++TmGRV//NVq93c3ik=";
                 //To require server to return authorization code
                 options.ResponseType = "code";
                 //To persist the tokens from Identity Server in the cookie
                 options.SaveTokens = true;
                 //The resource scopes (parts of the API) that you are requesting
                 //permission for to access
                 options.Scope.Add("IdentityServerApi");
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            });        }
    }
}
