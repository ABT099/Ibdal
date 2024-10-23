namespace Ibdal.Api.Forms.Validation;

public class CreateRepairFormValidation : AbstractValidator<CreateRepairForm>
{
    public CreateRepairFormValidation()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Comment).NotEmpty();
        RuleFor(x => x.RepairItems.Count).GreaterThan(0);
    }
}