namespace Ibdal.Api.Forms.Validation;

public class UpdateMessageFormValidation : AbstractValidator<UpdateMessageForm>
{
    public UpdateMessageFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Text).NotEmpty();

    }
}