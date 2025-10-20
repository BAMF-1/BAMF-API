// BAMF_API/Interfaces/ProductInterfaces/IVariantRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BAMF_API.Models;

namespace BAMF_API.Interfaces.ProductInterfaces
{
	public interface IVariantRepository
	{
		Task<Variant?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task GetBySkuAsync(string sku, object ct);
        Task<List<Variant>> ListByGroupAsync(Guid productGroupId, CancellationToken ct = default);
	}
}
