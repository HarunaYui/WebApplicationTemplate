using Dapper.Contrib.Extensions;
using WebApplicationTemplate.Model.Enums;

namespace WebApplicationTemplate.Model.Entity;

/// <summary>
/// 用户表
/// </summary>
[Table("user")]
public class User
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Key]
    public int ID { get; set; } = -1;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 密码用加密盐
    /// </summary>
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 用户身份
    /// </summary>
    public UserRole Role { get; set; } = UserRole.用户;
}

