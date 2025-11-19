using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Models;

namespace BAMF_API.Interfaces.ReviewInterfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync(int page);
        Task<int> GetReviewsCountAsync();
        Task<Review?> GetReviewAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProductGroupIdAsync(Guid productGroupId);
        Task<IEnumerable<Review>> GetReviewsByProductGroupSlugAsync(string slug);
        Task CreateReviewAsync(ReviewCreateDto dto);
        Task UpdateReviewAsync(int id, ReviewUpdateDto dto);
        Task DeleteReviewAsync(int id);
    }
}