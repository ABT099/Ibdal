namespace Ibdal.Api.Forms.Validation;

public class CreateOilChangeFormValidation : AbstractValidator<CreateOilChangeForm>
{
    public CreateOilChangeFormValidation()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.NextCarMeter).GreaterThan(0);
    }
}