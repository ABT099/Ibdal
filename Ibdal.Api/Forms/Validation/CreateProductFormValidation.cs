namespace Ibdal.Api.Forms.Validation;

public class CreateProductFormValidation : AbstractValidator<CreateProductForm>
{
    public CreateProductFormValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Image).NotNull();
    }
}