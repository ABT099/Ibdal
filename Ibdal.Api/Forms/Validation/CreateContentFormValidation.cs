namespace Ibdal.Api.Forms.Validation;

public class CreateContentFormValidation : AbstractValidator<CreateContentForm>
{
    public CreateContentFormValidation()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
    }
}