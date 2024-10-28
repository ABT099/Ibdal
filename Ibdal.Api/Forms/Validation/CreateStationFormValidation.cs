namespace Ibdal.Api.Forms.Validation;

public class CreateStationFormValidation : AbstractValidator<CreateStationForm>
{
    public CreateStationFormValidation()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
    }
}