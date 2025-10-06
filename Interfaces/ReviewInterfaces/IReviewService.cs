using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Models;

namespace BAMF_API.Interfaces.ReviewInterfaces
{
    public interface IReviewService
    {
        Task<Review?> GetReviewAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByItemIdAsync(int productId);
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task CreateReviewAsync(ReviewCreateDto dto);
        Task UpdateReviewAsync(int id, ReviewUpdateDto dto);
        Task DeleteReviewAsync(int id);
    }
}
