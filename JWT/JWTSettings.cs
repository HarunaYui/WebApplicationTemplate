namespace WebApplicationTemplate.JWT;

/// <summary>
/// JWT映射表
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string SecrectKey { get; set; } = string.Empty;

    /// <summary>
    /// 访问时间
    /// </summary>
    public int ExpireSeconds { get; set; } = 0;
}

