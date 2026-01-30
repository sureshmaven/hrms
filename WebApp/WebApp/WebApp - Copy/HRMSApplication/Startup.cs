using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using HRMSApplication.AuthHelpers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

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
    }
}