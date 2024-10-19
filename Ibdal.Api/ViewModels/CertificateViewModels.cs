namespace Ibdal.Api.ViewModels;

public static class CertificateViewModels
{
    public static Expression<Func<Certificate, object>> FlatProjection =>
        categoryItem => new
        {
            categoryItem.Id,
            categoryItem.Name,
            categoryItem.Description,
            CoverImageUrl = categoryItem.ImagesUrls.FirstOrDefault()
        };
    
    public static Expression<Func<Certificate, object>> Projection =>
        categoryItem => new
        {
            categoryItem.Id,
            categoryItem.Name,
            categoryItem.Description,
            categoryItem.ImagesUrls
        };
}