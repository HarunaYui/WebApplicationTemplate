using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;

namespace TestWebApplication.Entity;

public class User
{
    [Key]
    public int ID { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

