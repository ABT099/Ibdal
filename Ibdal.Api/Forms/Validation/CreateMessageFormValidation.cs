namespace Ibdal.Api.Forms.Validation;

public class CreateMessageFormValidation : AbstractValidator<CreateMessageForm>
{
    public CreateMessageFormValidation()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
    }
}