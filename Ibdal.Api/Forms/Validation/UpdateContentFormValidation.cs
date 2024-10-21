namespace Ibdal.Api.Forms.Validation;

public class UpdateContentFormValidation : AbstractValidator<UpdateContentForm>
{
    public UpdateContentFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
    }
}