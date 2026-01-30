using HRMSBusiness.Business;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace HRMSApplication.AuthHelpers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); // 
          
        }
    
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            LoginBus lbus = new LoginBus();
            LoginResult resLogin = lbus.ValidateUnmPwdForMobile(context.UserName, context.Password);

            if (resLogin.Success)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, resLogin.EmployeeFullName));
                identity.AddClaim(new Claim("empid", resLogin.EmpId));
                identity.AddClaim(new Claim("pkid", resLogin.EmpPkId.ToString()));
                //identity.AddClaim(new Claim("branchid", resLogin.Branch.ToString()));                
                identity.AddClaim(new Claim("desigcode", resLogin.desigcode.ToString()));
                context.Validated(identity);
            }
            else
            {
                context.SetError(resLogin.Message);
                //context.Rejected();
            }

            //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            //if (context.UserName == "rk" && context.Password == "pwd")
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Raju K"));
            //    identity.AddClaim(new Claim("empid", "742"));
            //    identity.AddClaim(new Claim("branchid", "43"));
            //    identity.AddClaim(new Claim("designationid", "11"));

            //    //context.Validated(identity);
            //    var props = new AuthenticationProperties(new Dictionary<string, string>
            //    {
            //    });

            //    //{
            //    //    "username", "rk"
            //    //                    },
            //    //                    {
            //    //    ClaimTypes.Name, "Raju K"
            //    //                    }

            //    var ticket = new AuthenticationTicket(identity, props);
            //    context.Validated(ticket);
            //}
            //else
            //{
            //    context.SetError("invalid_grant", "Provided username and password are incorrect");
            //    context.Rejected();
            //}

            //#region commented_Code
            ////using (var db = new TESTEntities())
            ////{
            ////    if (db != null)
            ////    {
            ////        var empl = db.Employees.ToList();
            ////        var user = db.Users.ToList();
            ////        if (user != null)
            ////        {
            ////            if (!string.IsNullOrEmpty(user.Where(u => u.UserName == context.UserName && u.Password == context.Password).FirstOrDefault().Name))
            ////            {
            ////                identity.AddClaim(new Claim("Age", "16"));

            ////                var props = new AuthenticationProperties(new Dictionary<string, string>
            ////                {
            ////                    {
            ////                        "userdisplayname", context.UserName
            ////                    },
            ////                    {
            ////                         "role", "admin"
            ////                    }
            ////                 });

            ////                var ticket = new AuthenticationTicket(identity, props);
            ////                context.Validated(ticket);                           
            ////            }
            ////            else
            ////            {
            ////                context.SetError("invalid_grant", "Provided username and password is incorrect");
            ////                context.Rejected();
            ////            }
            ////        }
            ////    }
            ////    else
            ////    {
            ////        context.SetError("invalid_grant", "Provided username and password is incorrect");
            ////        context.Rejected();
            ////    }
            ////    return;
            ////}
            //#endregion


        }
    }
}