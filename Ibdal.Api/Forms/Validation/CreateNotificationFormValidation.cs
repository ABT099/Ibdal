namespace Ibdal.Api.Forms.Validation;

public class CreateNotificationFormValidation : AbstractValidator<CreateNotificationForm>
{
    public CreateNotificationFormValidation()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x)
            .Must(form => form.StationsIds.Any() || form.UsersIds.Any());
    }
}