using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EraXP_Back.Models.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace EraXP_Back.Utils;

public class UserClaimUtils
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly Encoding _encoding;
    private readonly string _issuer;
    
    public UserClaimUtils(string key, string issuer, string encoding)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        this._symmetricSecurityKey = new SymmetricSecurityKey(keyBytes);
        this._signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512);
        this._encoding = Encoding.GetEncoding(encoding);
        this._issuer = issuer;
    }
    
    public string GenerateJsonSignature(UserClaims userClaims)
    {
        List<Claim> claims = new List<Claim>();
        
        claims.Add(new Claim("sub", userClaims.Username));
        claims.Add(new Claim("securityToken", userClaims.SecurityToken.ToString()));

        ClaimsIdentity identity = new ClaimsIdentity(new[]
        {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userClaims.Username),
            new Claim("securityToken", userClaims.SecurityToken.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _issuer,
            Audience = _issuer,
            SigningCredentials = _signingCredentials
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string jwtToken = tokenHandler.WriteToken(token);
        
        return jwtToken;
    }

    public UserClaims GetClaims(IEnumerable<Claim> claims)
    {
        string? username = null;
        Guid? securityToken = null;
        
        foreach (var claim in claims)
        {
            switch (claim.Type)
            {
                case "sub":
                    username = claim.Value;
                    break;
                case "securityToken":
                    securityToken = Guid.Parse(claim.Value);
                    break;
            }
        }

        if (username == null || securityToken == null)
            throw new ArgumentException("Missing or invalid claims!");
        
        UserClaims? userClaims = new UserClaims(username, securityToken.Value);

        return userClaims;
    }
}