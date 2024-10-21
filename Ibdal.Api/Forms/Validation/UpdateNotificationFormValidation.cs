namespace Ibdal.Api.Forms.Validation;

public class UpdateNotificationFormValidation : AbstractValidator<UpdateNotificationForm>
{
    public UpdateNotificationFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x)
            .Must(form => form.StationsIds.Any() || form.UsersIds.Any());
    }
}