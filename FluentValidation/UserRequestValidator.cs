using FluentValidation;
using WebApplicationTemplate.Entity;

namespace WebApplicationTemplate.FluentValidation;

public class UserRequestValidator: AbstractValidator<User>
{
    public UserRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("名字不能为空");
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r=>r.Password).NotEmpty().MinimumLength(12).WithMessage("最小12位字符");
    }
}

