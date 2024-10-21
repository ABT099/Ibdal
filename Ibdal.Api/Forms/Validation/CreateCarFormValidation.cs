
namespace Ibdal.Api.Forms.Validation;

public class CreateCarFormValidation : AbstractValidator<CreateCarForm>
{
    public CreateCarFormValidation()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.PlateNumber).NotEmpty();
        RuleFor(x => x.ChaseNumber).NotEmpty();
        RuleFor(x => x.CarType).NotEmpty();
        RuleFor(x => x.CarModel).NotEmpty();
    }
}