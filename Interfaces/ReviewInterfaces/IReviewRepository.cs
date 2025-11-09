using BAMF_API.Models;

namespace BAMF_API.Interfaces.ReviewInterfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByItemIdAsync(int productId);
        Task<IEnumerable<Review>> GetAllAsync(int page);
        Task<int> GetReviewsCountAsync();
        Task CreateAsync(Review Review);
        Task UpdateAsync(Review Review);
        Task DeleteAsync(int id);
    }
}
