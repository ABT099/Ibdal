namespace Ibdal.Api.Forms.Validation;

public class UpdateCertificateFormValidation : AbstractValidator<UpdateCertificateForm>
{
    public UpdateCertificateFormValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}