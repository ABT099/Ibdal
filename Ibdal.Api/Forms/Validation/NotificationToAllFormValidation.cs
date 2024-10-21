namespace Ibdal.Api.Forms.Validation;

public class NotificationToAllFormValidation : AbstractValidator<NotificationToAllForm>
{
    public NotificationToAllFormValidation()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}