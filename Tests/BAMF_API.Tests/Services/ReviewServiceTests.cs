using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Interfaces.ReviewInterfaces;
using BAMF_API.Models;
using BAMF_API.Services;
using Moq;
using Xunit;

namespace BAMF_API.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _repoMock;
        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _repoMock = new Mock<IReviewRepository>();
            _service = new ReviewService(_repoMock.Object);
        }

        [Fact]
        // Tests that GetAllReviewsAsync returns a list of reviews
        public async Task GetAllReviewsAsync_ShouldReturnReviews()
        {
            var reviews = new List<Review> { new Review { Id = 1, Rating = 5, Title = "Great!" } };
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<int>())).ReturnsAsync(reviews);

            var result = await _service.GetAllReviewsAsync(1);

            Assert.Single(result);
            Assert.Equal("Great!", ((List<Review>)result)[0].Title);
        }

        [Fact]
        // Tests that GetReviewsCountAsync returns the correct count
        public async Task GetReviewsCountAsync_ShouldReturnCount()
        {
            _repoMock.Setup(r => r.GetReviewsCountAsync()).ReturnsAsync(42);

            var result = await _service.GetReviewsCountAsync();

            Assert.Equal(42, result);
        }

        [Fact]
        // Tests that GetReviewAsync returns a review if it exists
        public async Task GetReviewAsync_ShouldReturnReview_WhenExists()
        {
            var review = new Review { Id = 1, Rating = 5 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            var result = await _service.GetReviewAsync(1);

            Assert.NotNull(result);
            Assert.Equal(5, result.Rating);
        }

        [Fact]
        // Tests that GetReviewsByProductGroupIdAsync returns correct reviews
        public async Task GetReviewsByProductGroupIdAsync_ShouldReturnReviews()
        {
            var productGroupId = Guid.NewGuid();
            var reviews = new List<Review> { new Review { Id = 1, ProductGroupId = productGroupId } };
            _repoMock.Setup(r => r.GetByProductGroupIdAsync(productGroupId)).ReturnsAsync(reviews);

            var result = await _service.GetReviewsByProductGroupIdAsync(productGroupId);

            Assert.Single(result);
            Assert.Equal(productGroupId, ((List<Review>)result)[0].ProductGroupId);
        }

        [Fact]
        // Tests that GetReviewsByProductGroupSlugAsync returns correct reviews
        public async Task GetReviewsByProductGroupSlugAsync_ShouldReturnReviews()
        {
            var slug = "example-slug";
            var reviews = new List<Review> { new Review { Id = 1, Title = "Test" } };
            _repoMock.Setup(r => r.GetByProductGroupSlugAsync(slug)).ReturnsAsync(reviews);

            var result = await _service.GetReviewsByProductGroupSlugAsync(slug);

            Assert.Single(result);
            Assert.Equal("Test", ((List<Review>)result)[0].Title);
        }

        [Fact]
        // Tests that CreateReviewAsync calls the repository create method
        public async Task CreateReviewAsync_ShouldCallRepoCreate()
        {
            var dto = new ReviewCreateDto
            {
                ProductGroupId = Guid.NewGuid(),
                Rating = 4,
                Title = "New Review",
                Comment = "Nice!"
            };

            await _service.CreateReviewAsync(dto);

            _repoMock.Verify(r => r.CreateAsync(It.Is<Review>(rev =>
                rev.Title == "New Review" &&
                rev.Rating == 4 &&
                rev.Comment == "Nice!" &&
                rev.ProductGroupId == dto.ProductGroupId
            )), Times.Once);
        }

        [Fact]
        // Tests that UpdateReviewAsync correctly updates an existing review
        public async Task UpdateReviewAsync_ShouldUpdateReview_WhenExists()
        {
            var existingReview = new Review { Id = 1, Rating = 3, Title = "Old", Comment = "Old comment" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingReview);

            var dto = new ReviewUpdateDto { Rating = 5, Title = "Updated", Comment = "Updated comment" };
            await _service.UpdateReviewAsync(1, dto);

            _repoMock.Verify(r => r.UpdateAsync(It.Is<Review>(rev =>
                rev.Id == 1 &&
                rev.Rating == 5 &&
                rev.Title == "Updated" &&
                rev.Comment == "Updated comment"
            )), Times.Once);
        }

        [Fact]
        // Tests that UpdateReviewAsync throws if the review does not exist
        public async Task UpdateReviewAsync_ShouldThrow_WhenReviewNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Review)null);
            var dto = new ReviewUpdateDto { Rating = 5 };

            await Assert.ThrowsAsync<Exception>(() => _service.UpdateReviewAsync(1, dto));
        }

        [Fact]
        // Tests that DeleteReviewAsync calls the repository delete method if review exists
        public async Task DeleteReviewAsync_ShouldCallRepoDelete_WhenReviewExists()
        {
            var existingReview = new Review { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingReview);

            await _service.DeleteReviewAsync(1);

            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        // Tests that DeleteReviewAsync throws if the review does not exist
        public async Task DeleteReviewAsync_ShouldThrow_WhenReviewNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Review)null);

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteReviewAsync(1));
        }
    }
}
