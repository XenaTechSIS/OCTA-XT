using System;
using System.Web.Security;

namespace FSP.Web.Helpers
{
    public static class MembershipExtensions
    {
        public static Guid GetUserId()
        {
            var result = Guid.Empty;
            try
            {
                var myUser = Membership.GetUser();
                if (myUser?.ProviderUserKey != null)
                    Guid.TryParse(myUser.ProviderUserKey.ToString(), out result);
            }
            catch
            {
                // ignored
            }

            return result;
        }
    }
}