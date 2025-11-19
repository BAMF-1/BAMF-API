using BAMF_API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.SlugURLInterfaces
{ 
	public interface ISlugService
	{
		string GenerateGroupSlug(string name);
		Task<string> EnsureUniqueGroupSlugAsync(string baseSlug, CancellationToken ct = default);
	}
}
