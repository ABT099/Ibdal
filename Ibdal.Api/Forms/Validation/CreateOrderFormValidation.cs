namespace Ibdal.Api.Forms.Validation;

public class CreateOrderFormValidation : AbstractValidator<CreateOrderForm>
{
    public CreateOrderFormValidation()
    {
        RuleFor(x => x.StationId).NotEmpty();
        RuleFor(x => x.Products.Count()).GreaterThan(0);
    }
}