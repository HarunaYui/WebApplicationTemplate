using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace WebApplicationTemplate.Entity;

/// <summary>
/// 用户表
/// </summary>
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
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

