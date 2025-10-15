using System;

namespace BAMF_API.Services;

public class UrlBuilder : IUrlBuilder
{
    private readonly string _baseUrl;
    public UrlBuilder(string? baseUrl = null)
    {
        _baseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
    }

    public string GroupUrl(string slugOrObjectId, string? sku = null)
    {
        if (string.IsNullOrWhiteSpace(slugOrObjectId)) throw new ArgumentNullException(nameof(slugOrObjectId));
        var url = $"/products/{slugOrObjectId}";
        if (!string.IsNullOrWhiteSpace(sku)) url += $"?sku={Uri.EscapeDataString(sku)}";
        return PrependBase(url);
    }

    public string VariantRedirectUrl(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentNullException(nameof(sku));
        var url = $"/p/{Uri.EscapeDataString(sku)}";
        return PrependBase(url);
    }

    private string PrependBase(string path) => string.IsNullOrEmpty(_baseUrl) ? path : $"{_baseUrl}{path}";
}