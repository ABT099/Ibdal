namespace Ibdal.Api.Forms.Validation;

public class CreateCategoryItemFormValidation : AbstractValidator<CreateCategoryItemForm>
{
    public CreateCategoryItemFormValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Image).NotNull();
    }
}