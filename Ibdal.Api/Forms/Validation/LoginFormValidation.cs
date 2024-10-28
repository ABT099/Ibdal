namespace Ibdal.Api.Forms.Validation;

public class LoginFormValidation : AbstractValidator<LoginForm>
{
    public LoginFormValidation()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}