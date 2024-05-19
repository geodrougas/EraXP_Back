using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Domain.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace EraXP_Back.Utils;

public class UserClaimUtils
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly Encoding _encoding;
    private readonly string _issuer;
    public TimeSpan AuthLifetime { get; }

    public UserClaimUtils(string key, string issuer, string encoding, TimeSpan authLifetime)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        this._symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);
        this._signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512);
        this._encoding = Encoding.GetEncoding(encoding);
        this._issuer = issuer;
        AuthLifetime = authLifetime;
    }
    
    public string GenerateJsonSignature(ClaimsPrincipal principal)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = principal.Identity as ClaimsIdentity,
            Expires = DateTime.UtcNow.Add(AuthLifetime),
            Issuer = _issuer,
            Audience = _issuer,
            SigningCredentials = _signingCredentials
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string jwtToken = tokenHandler.WriteToken(token);
        
        return jwtToken;
    }

    public Authority? GetAuthority(ClaimsPrincipal? principal)
    {
        List<Claim> claims;
        if (principal == null || principal.Identity == null || (claims = principal.Claims.ToList()).Count == 0)
            return null;

        string? id = null;
        string? key = null;
        string? authorizationScheme = null;
        bool hadValidAuthorityType = false;
        EAuthorityType authorityType = EAuthorityType.None;
        
        int rolesLength = 0;
        string[] roles = new string[claims.Count];
        
        foreach (var claim in claims)
        {
            switch (claim.Type)
            {
                case "type":
                    authorizationScheme = null;
                    break;
                case ClaimTypes.NameIdentifier:
                    id = claim.Value;
                    break;
                case "key":
                    key = claim.Value;
                    break;
                case ClaimTypes.Role:
                    roles[rolesLength++] = claim.Value;
                    break;
                case "authority_type":
                    hadValidAuthorityType = Enum.TryParse(claim.Value, true, out authorityType);
                    break;
            }
        }

        if (id == null || key == null || !hadValidAuthorityType || authorizationScheme == null)
            throw new ArgumentException("Missing or invalid claims!");
        
        Array.Resize(ref roles, rolesLength);
        
        return new Authority(authorizationScheme, id, key, authorityType, roles);
    }

    public ClaimsPrincipal GetPrincipal(Authority authority)
    {
        ClaimsIdentity identity = new ClaimsIdentity(new[]
        {
            new Claim("type", authority.AuthorizationScheme),
            new Claim(ClaimTypes.NameIdentifier, authority.Id),
            new Claim("key", authority.Key),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("authority_type", authority.Type.ToString())
        });
        foreach (var role in authority.Roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsPrincipal(identity);
    }
}