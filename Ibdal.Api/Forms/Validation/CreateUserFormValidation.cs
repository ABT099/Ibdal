namespace Ibdal.Api.Forms.Validation;

public class CreateUserFormValidation : AbstractValidator<CreateUserForm>
{
    public CreateUserFormValidation()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}