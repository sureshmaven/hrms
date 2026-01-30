using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using HRMSApplication.AuthHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(HRMSApplication.Startup))]
namespace HRMSApplication
{    
    public class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            string time = System.Configuration.ConfigurationManager.AppSettings["APITimeOut"];
            double timeout = Convert.ToDouble(time);
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/mobile/gentoken"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(timeout),
                Provider = new SimpleAuthorizationServerProvider()
            };

            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
        
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        //cors
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddCors(options =>
        //    {
        //        options.AddDefaultPolicy(
        //           builder =>
        //           {
        //               //builder.WithOrigins("http://localhost:63538", "http://localhost:3000")
        //               builder.WithOrigins("http://192.168.0.68:8089")
        //               .AllowAnyHeader()
        //               .AllowAnyMethod();
        //           }
        //           );
        //    });


        //}

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    app.UseCors();
        //    app.UseCors(builder =>
        //    {
        //        builder
        //        .WithOrigins("http://192.168.0.68:8089")
        //        .AllowAnyHeader()
        //        .AllowAnyMethod();
        //    });

        //}
        //        public void ConfigureServices(IServiceCollection services)
        //        {
        //            services.AddSession(options =>
        //            {
        //                options.IdleTimeout = TimeSpan.FromMinutes(20);
        //                options.Cookie.HttpOnly = true;
        //                options.Cookie.IsEssential = true;
        //            });

        //            services.AddControllersWithViews();
        //        }
        //        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    app.UseRouting();

        //    app.UseSession();  // Enable session support
        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Home}/{action=Index}/{id?}");
        //    });
        //}
    }
}