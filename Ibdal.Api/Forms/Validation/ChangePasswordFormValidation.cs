namespace Ibdal.Api.Forms.Validation;

public class ChangePasswordFormValidation : AbstractValidator<ChangePasswordForm>
{
    public ChangePasswordFormValidation()
    {
        RuleFor(x => x.AuthId).NotEmpty();
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .NotEqual(x => x.NewPassword);
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}