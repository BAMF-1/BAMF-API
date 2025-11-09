using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Interfaces.ReviewInterfaces;
using BAMF_API.Models;

namespace BAMF_API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _ReviewRepo;

        public ReviewService(IReviewRepository ReviewRepo)
        {
            _ReviewRepo = ReviewRepo;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync(int page)
        {
            return await _ReviewRepo.GetAllAsync(page);
        }

        public async Task<int> GetReviewsCountAsync()
        {
            return await _ReviewRepo.GetReviewsCountAsync();
        }

        public async Task<Review?> GetReviewAsync(int id)
        {
            return await _ReviewRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Review>> GetReviewsByItemIdAsync(int productId)
        {
            return await _ReviewRepo.GetByItemIdAsync(productId);
        }

        public async Task CreateReviewAsync(ReviewCreateDto dto)
        {
            // TODO: Check for product before adding review, or it will throw error
            var review = new Review
            {
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                Title = dto.Title
            };
            await _ReviewRepo.CreateAsync(review);
        }

        public async Task UpdateReviewAsync(int id, ReviewUpdateDto dto)
        {
            // TODO: Check for product before updating review, or it will throw error
            var existingReview = await _ReviewRepo.GetByIdAsync(id);
            if (existingReview == null)
                throw new Exception("Review not found");
            if (existingReview.Rating != dto.Rating)
                existingReview.Rating = dto.Rating;
            if (!string.IsNullOrWhiteSpace(dto.Comment))
                existingReview.Comment = dto.Comment;
            if (!string.IsNullOrEmpty(dto.Title))
                existingReview.Title = dto.Title;
            existingReview.UpdatedUtc = DateTime.UtcNow;
            await _ReviewRepo.UpdateAsync(existingReview);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var existingReview = await _ReviewRepo.GetByIdAsync(id);
            if (existingReview == null)
                throw new Exception("Review not found");
            await _ReviewRepo.DeleteAsync(id);
        }
    }
}