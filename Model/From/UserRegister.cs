using FluentValidation;

namespace WebApplicationTemplate.Model.From;

/// <summary>
/// 用户注册
/// </summary>
public class UserRegister
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string PassWord { get; set; } = string.Empty;
    
}

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
        RuleFor(r => r.PassWord).NotEmpty().MustAsync(async (x, _) => x.Length >= 8).WithMessage("长度不能小于8");
    }
}

