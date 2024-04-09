namespace TestWebApplication.JWT;

public class JWTSettings
{
    public string SecrectKey { get; set; } = string.Empty;
    public int ExpireSeconds { get; set; } = 0;
}

