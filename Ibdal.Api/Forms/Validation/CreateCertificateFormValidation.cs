namespace Ibdal.Api.Forms.Validation;

public class CreateCertificateFormValidation : AbstractValidator<CreateCertificateForm>
{
    public CreateCertificateFormValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Images.Count()).GreaterThan(0);
    }
}