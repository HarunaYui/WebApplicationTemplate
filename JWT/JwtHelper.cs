using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplicationTemplate.JWT;

/// <summary>
/// JWT帮助
/// </summary>
public class JwtHelper
{
    /// <summary>
    /// 创建Jwt
    /// </summary>
    /// <param name="claims"></param>
    /// <param name="key"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    public static string JwtCreate(List<Claim> claims,string key,DateTime expire)
    {
        byte[] secBytes = Encoding.UTF8.GetBytes(key);
        var secKey = new SymmetricSecurityKey(secBytes);
        var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor =
            new JwtSecurityToken(claims: claims, expires: expire, signingCredentials: credentials);
        string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return jwt;
    }

    public static string JwtAddAdmin(List<Claim> claims, string key, DateTime expire)
    {
        claims.Add(new(ClaimTypes.Role, "admin"));
        var result = JwtCreate(claims, key, expire);
        return result;
    }
}

