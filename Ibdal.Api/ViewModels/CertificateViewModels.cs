namespace Ibdal.Api.ViewModels;

public static class CertificateViewModels
{
    public static Expression<Func<Certificate, object>> FlatProjection =>
        certificate => new
        {
            certificate.Id,
            certificate.Name,
            certificate.Description,
            CoverImageUrl = certificate.ImagesUrls.FirstOrDefault()
        };
    
    public static Expression<Func<Certificate, object>> Projection =>
        certificate => new
        {
            certificate.Id,
            certificate.Name,
            certificate.Description,
            certificate.ImagesUrls
        };
}