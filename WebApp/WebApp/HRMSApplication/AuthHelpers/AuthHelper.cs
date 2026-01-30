using System.Security.Claims;
using System.Security.Principal;

namespace HRMSApplication.AuthHelpers
{
    public class AuthHelper
    {
        public static UserInfo LoggedinUserInfo(IPrincipal user)
        {
            var identity = (ClaimsIdentity)user.Identity;

            UserInfo uInfo = new UserInfo();
            uInfo.Name = identity.Name;

            foreach (Claim cl in identity.Claims)
            {
                if (cl.Type == "empid")
                    uInfo.EmpId = int.Parse(cl.Value);
                else if (cl.Type == "pkid")
                    uInfo.PkId = int.Parse(cl.Value);
                //else if (cl.Type == "branchid")
                //    uInfo.BranchId = int.Parse(cl.Value);
                else if (cl.Type == "desigcode")
                    uInfo.desigcode = cl.Value;
            }

            return uInfo;
        }
    }
}