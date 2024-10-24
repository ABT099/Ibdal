namespace Ibdal.Api.Forms.Validation;

public class UpdateStationValidation : AbstractValidator<UpdateStationForm>
{
    public UpdateStationValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
    }
}