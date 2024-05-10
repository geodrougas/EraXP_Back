using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EraXP_Back.Models.Domain;
using Microsoft.AspNetCore.Authentication;

namespace EraXP_Back.Utils;

public class UserClaimUtils
{
    private const string HEADER = """{"alg":"HS512","typ":"JWT"}""";
    private readonly string HEADER_BASE64;
    private string _key;
    private HMACSHA512 _hmacsha512;
    private Encoding _encoding;
    
    public UserClaimUtils(string key, string encoding)
    {
        byte[] keyBytes = Convert.FromBase64String(key);
        this._key = key;
        this._hmacsha512 = new HMACSHA512(keyBytes); 
        this._encoding = Encoding.GetEncoding(encoding);
        this.HEADER_BASE64 = StringToBase64(HEADER);
    }
    
    public string GenerateJsonSignature(UserClaims claims)
    {
        string payload = JsonSerializer.Serialize(claims);
        string headerBase64 = HEADER_BASE64;
        string payloadBase64 = StringToBase64(payload);
        string signatureContent = $"{headerBase64}.{payloadBase64}";
        string signatureBase64 = ComputeSignature(signatureContent);
        return $"{signatureContent}.{signatureBase64}";
    }

    public bool ValidateSignature(string jwt)
    {
        string[] parts = jwt.Split(".");

        return ValidateSignature(parts);
    }

    public bool ValidateSignature(string[] jwtParts)
    {
        if (jwtParts.Length != 3)
            return false;

        string signatureContent = $"{jwtParts[0]}.{jwtParts[1]}";
        return jwtParts[2] == ComputeSignature(signatureContent);
    }

    public UserClaims GetClaims(string token)
    {
        string[] parts = token.Split(".");

        bool isValid = ValidateSignature(parts);

        if (isValid)
            throw new UnauthorizedAccessException("Invalid token signature!");

        UserClaims? claims = JsonSerializer.Deserialize<UserClaims>(parts[1]);

        if (claims == null)
            throw new UnauthorizedAccessException("Unable to extract user claims!");

        return claims;
    }
    
    public string ComputeSignature(string content)
    {
        byte[] signatureBytes = _hmacsha512.ComputeHash(_encoding.GetBytes(content));
        return Base64UrlTextEncoder.Encode(signatureBytes);
    }
    
    public string StringToBase64(string text)
    {
        byte[] textBytes = _encoding.GetBytes(text);
        return Base64UrlTextEncoder.Encode(textBytes);
    }
}