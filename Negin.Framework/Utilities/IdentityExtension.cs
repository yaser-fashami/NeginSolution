
using System.Security.Claims;
using System.Security.Principal;

namespace Negin.Framework.Utilities;

public static class IdentityExtension
{
    public static string GetCurrentUserId(this IIdentity identity)
    {
        if (identity == null)
            return string.Empty;

        var claimsIdentity = identity as ClaimsIdentity;

        var userDataClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (userDataClaim == null || userDataClaim.Value == null)
            return string.Empty;

        var userId = userDataClaim.Value;

        return string.IsNullOrWhiteSpace(userId) ? string.Empty : userId;
    }

    public static string GetCurrentUserEmail(this IIdentity identity)
    {
        if (identity == null)
            return string.Empty;

        var claimsIdentity = identity as ClaimsIdentity;

        var userDataClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (userDataClaim == null || userDataClaim.Value == null)
            return string.Empty;

        var userEmail = userDataClaim.Value;

        return string.IsNullOrWhiteSpace(userEmail) ? string.Empty : userEmail;
    }

}
