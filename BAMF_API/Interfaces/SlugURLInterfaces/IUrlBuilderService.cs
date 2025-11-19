using BAMF_API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.SlugURLInterfaces
{ 
	public interface IUrlBuilderService
	{
		string GroupUrl(string slugOrObjectId, string? sku = null);
		string VariantRedirectUrl(string sku);
	}
}