namespace Ibdal.Api.Forms.Validation;

public class CreateCategoryFormValidation : AbstractValidator<CreateCategoryForm>
{
    public CreateCategoryFormValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}