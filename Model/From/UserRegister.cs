using FluentValidation;

namespace WebApplicationTemplate.Model.From;

/// <summary>
/// 用户注册
/// </summary>
public record UserRegister(string UserName, string PassWord);

/// <summary>
/// 用户注册验证
/// </summary>
public class UserRegisterValidator : AbstractValidator<UserRegister>
{
    /// <summary>
    /// 方法
    /// </summary>
    public UserRegisterValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("名字不能为空");
        RuleFor(r => r.PassWord).NotEmpty().Must(x => x.Length >= 8).WithMessage("长度不能小于8");
    }
}

