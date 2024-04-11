using Dapper;
using FluentValidation;
using MySqlConnector;
using WebApplicationTemplate.Entity;

namespace WebApplicationTemplate.FluentValidation;

/// <summary>
/// 用户数据验证
/// </summary>
public class UserRequestValidator: AbstractValidator<User>
{
    private readonly MySqlConnection? _connection;

    /// <summary>
    /// 构造注入数据库
    /// </summary>
    /// <param name="connection"></param>
    public UserRequestValidator(MySqlConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// 数据验证
    /// </summary>
    public UserRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("名字不能为空");
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MustAsync(async (x,_) =>
        {
            var result = await _connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM user WHERE Email = @Email", new { Email = x });
            if (result is null)
            {
                return false;
            }
            return true;
        }).WithMessage(c=>$"{c.Email}已经存在");
        RuleFor(r=>r.Password).NotEmpty().MinimumLength(12).WithMessage("最小12位字符");
    }
}

