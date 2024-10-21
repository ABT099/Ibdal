namespace Ibdal.Api.Forms.Validation;

public class UpdateCategoryFormValidation : AbstractValidator<UpdateCategoryForm>
{
    public UpdateCategoryFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}