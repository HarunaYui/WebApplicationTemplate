using System.Security.Cryptography;
using System.Text;

namespace WebApplicationTemplate.Model;

/// <summary>
/// 密码加密
/// </summary>
public class UserPasswordSet
{
    /// <summary>
    /// 加密密码创建
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static string SaltedPassword(string password, string salt)
    {
        return BitConverter.ToString(SHA1.Create().ComputeHash(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(password + salt)))).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 创建盐
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public static string CreateSalt(string userName)
    {
        // 取中间的 8 位
        return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(userName + (DateTime.Now.Ticks - 621355968000000000).ToString()))).Replace("-", "").ToLower().Substring(8, 8);
    }
}

