namespace Ibdal.Api.Forms.Validation;

public class UpdateUserFormValidation : AbstractValidator<UpdateUserForm>
{
    public UpdateUserFormValidation()
    {       
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}