namespace Ibdal.Api.Forms.Validation;

public class UpdateProductFormValidation : AbstractValidator<UpdateProductForm>
{
    public UpdateProductFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}