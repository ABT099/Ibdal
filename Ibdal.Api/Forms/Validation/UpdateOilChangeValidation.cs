namespace Ibdal.Api.Forms.Validation;

public class UpdateOilChangeValidation : AbstractValidator<UpdateOilChangeForm>
{
    public UpdateOilChangeValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.NextCarMeter).GreaterThan(0);
    }
}