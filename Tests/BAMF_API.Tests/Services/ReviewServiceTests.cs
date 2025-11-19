using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Interfaces.ReviewInterfaces;
using BAMF_API.Models;
using BAMF_API.Services;
using Moq;
using Xunit;

namespace BAMF_API.Tests.BAMF_API.Tests.Services
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
        public async Task GetAllReviewsAsync_ShouldReturnReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { Id = 1, Rating = 5, Title = "Great!" }
            };
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<int>()))
                     .ReturnsAsync(reviews);

            // Act
            var result = await _service.GetAllReviewsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var review = result.First();
            Assert.Equal("Great!", review.Title);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldCallRepoCreate()
        {
            // Arrange
            var productGroupId = Guid.NewGuid();
            var dto = new ReviewCreateDto
            {
                ProductGroupId = productGroupId,
                Rating = 4,
                Title = "Test",
                Comment = "Test comment"
            };

            // Act
            await _service.CreateReviewAsync(dto);

            // Assert
            _repoMock.Verify(r => r.CreateAsync(
                It.Is<Review>(rev =>
                    rev.Title == "Test" &&
                    rev.Rating == 4 &&
                    rev.ProductGroupId == productGroupId &&
                    rev.Comment == "Test comment"
                )), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldUpdateExistingReview()
        {
            // Arrange
            var existingReview = new Review
            {
                Id = 1,
                Rating = 3,
                Title = "Old Title",
                Comment = "Old Comment",
                CreatedUtc = DateTime.UtcNow.AddDays(-1)
            };

            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(existingReview);

            var updateDto = new ReviewUpdateDto
            {
                Rating = 5,
                Title = "Updated Title",
                Comment = "Updated Comment"
            };

            // Act
            await _service.UpdateReviewAsync(1, updateDto);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(
                It.Is<Review>(rev =>
                    rev.Id == 1 &&
                    rev.Rating == 5 &&
                    rev.Title == "Updated Title" &&
                    rev.Comment == "Updated Comment" &&
                    rev.UpdatedUtc != null
                )), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldThrowException_WhenReviewNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((Review?)null);

            var updateDto = new ReviewUpdateDto { Rating = 5 };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.UpdateReviewAsync(999, updateDto)
            );
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldCallRepoDelete()
        {
            // Arrange
            var existingReview = new Review { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(existingReview);

            // Act
            await _service.DeleteReviewAsync(1);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldThrowException_WhenReviewNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((Review?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.DeleteReviewAsync(999)
            );
        }

        [Fact]
        public async Task GetReviewsByProductGroupIdAsync_ShouldReturnFilteredReviews()
        {
            // Arrange
            var productGroupId = Guid.NewGuid();
            var reviews = new List<Review>
            {
                new Review { Id = 1, ProductGroupId = productGroupId, Rating = 5 },
                new Review { Id = 2, ProductGroupId = productGroupId, Rating = 4 }
            };

            _repoMock.Setup(r => r.GetByProductGroupIdAsync(productGroupId))
                     .ReturnsAsync(reviews);

            // Act
            var result = await _service.GetReviewsByProductGroupIdAsync(productGroupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(productGroupId, r.ProductGroupId));
        }
    }
}