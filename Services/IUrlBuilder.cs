namespace BAMF_API.Services;

public interface IUrlBuilder
{
    string GroupUrl(string slugOrObjectId, string? sku = null);
    string VariantRedirectUrl(string sku);
}