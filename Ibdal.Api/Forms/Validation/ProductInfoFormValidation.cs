namespace Ibdal.Api.Forms.Validation;

public class ProductInfoFormValidation : AbstractValidator<ProductInfoForm>
{
    public ProductInfoFormValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}