namespace Ibdal.Api.Forms.Validation;

public class UpdateRepairFormValidation : AbstractValidator<UpdateRepairForm>
{
    public UpdateRepairFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}