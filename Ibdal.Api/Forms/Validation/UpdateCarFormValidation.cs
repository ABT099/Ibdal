namespace Ibdal.Api.Forms.Validation;

public class UpdateCarFormValidation : AbstractValidator<UpdateCarForm>
{
    public UpdateCarFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DriverName).NotEmpty();
        RuleFor(x => x.DriverPhoneNumber).NotEmpty();
        RuleFor(x => x.PlateNumber).NotEmpty();
        RuleFor(x => x.ChaseNumber).NotEmpty();
        RuleFor(x => x.CarType).NotEmpty();
        RuleFor(x => x.CarModel).NotEmpty();
    }
}