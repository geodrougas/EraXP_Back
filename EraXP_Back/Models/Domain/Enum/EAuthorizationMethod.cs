using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EraXP_Back.Models.Domain.Enum;

public enum EAuthorizationMethod
{
    JWT,
    Cookies
    
}

public class AuthorizationMethod
{
    public static string GetScheme(EAuthorizationMethod method)
    {
        switch (method)
        {
            case EAuthorizationMethod.Cookies: return CookieAuthenticationDefaults.AuthenticationScheme;
            case EAuthorizationMethod.JWT: return JwtBearerDefaults.AuthenticationScheme;
            default: throw new NotImplementedException();
        }
    }
}