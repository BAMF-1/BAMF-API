using System;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Interfaces.ProductInterfaces
{ 
    public interface IProductGroupRepository
    {
        Task<bool> ExistsByObjectIdAsync(string objectId, CancellationToken ct = default);
        Task<ProductGroup?> GetBySlugOrObjectIdAsync(string idOrSlug, CancellationToken ct = default);
    }
}