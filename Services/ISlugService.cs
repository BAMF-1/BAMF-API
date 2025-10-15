using System.Threading;
using System.Threading.Tasks;

namespace BAMF_API.Services;

public interface ISlugService
{
    string GenerateGroupSlug(string name);
    Task<string> EnsureUniqueGroupSlugAsync(string baseSlug, CancellationToken ct = default);
}