using Dapper;
using FluentValidation;
using WebApplicationTemplate.Entity;

namespace WebApplicationTemplate.FluentValidation;

public class UserRequestValidator: AbstractValidator<User>
{
    private AppDB.AppDB Db { get; }

    public UserRequestValidator(AppDB.AppDB db)
    {
        this.Db = db;
    }
    public UserRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("名字不能为空");
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MustAsync(async (x,_) =>
        {
            var result = await Db.Connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM user WHERE Email = @Email", new { Email = x });
            if (result is null)
            {
                return false;
            }
            return true;
        }).WithMessage(c=>$"{c.Email}已经存在");
        RuleFor(r=>r.Password).NotEmpty().MinimumLength(12).WithMessage("最小12位字符");
    }
}

