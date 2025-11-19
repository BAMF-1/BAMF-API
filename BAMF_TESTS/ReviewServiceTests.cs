namespace BAMF_API.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _mockRepo;
        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _mockRepo = new Mock<IReviewRepository>();
            _service = new ReviewService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllReviewsAsync_Returns_List()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync(1))
                .ReturnsAsync(new List<Review> { new Review { Id = 1 } });

            // Act
            var result = await _service.GetAllReviewsAsync(1);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetReviewAsync_Returns_Review()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Review { Id = 1 });

            var result = await _service.GetReviewAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetReviewsByProductGroupIdAsync_Returns_List()
        {
            var pgId = Guid.NewGuid();

            _mockRepo.Setup(r => r.GetByProductGroupIdAsync(pgId))
                .ReturnsAsync(new List<Review> { new Review { ProductGroupId = pgId } });

            var result = await _service.GetReviewsByProductGroupIdAsync(pgId);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetReviewsByProductGroupSlugAsync_Returns_List()
        {
            _mockRepo.Setup(r => r.GetByProductGroupSlugAsync("slug"))
                .ReturnsAsync(new List<Review> { new Review { Id = 1 } });

            var result = await _service.GetReviewsByProductGroupSlugAsync("slug");

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateReviewAsync_Calls_Repo_CreateAsync()
        {
            var dto = new ReviewCreateDto
            {
                ProductGroupId = Guid.NewGuid(),
                Title = "Test",
                Comment = "Good",
                Rating = 4
            };

            await _service.CreateReviewAsync(dto);

            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_Updates_Existing_Review()
        {
            var existing = new Review
            {
                Id = 1,
                Rating = 3,
                Comment = "Old",
                Title = "OldTitle"
            };

            var dto = new ReviewUpdateDto
            {
                Rating = 5,
                Comment = "New",
                Title = "NewTitle"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existing);

            await _service.UpdateReviewAsync(1, dto);

            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Review>(rev =>
                rev.Rating == 5 &&
                rev.Comment == "New" &&
                rev.Title == "NewTitle"
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_Throws_When_Not_Found()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            var dto = new ReviewUpdateDto();

            await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateReviewAsync(1, dto));
        }

        [Fact]
        public async Task DeleteReviewAsync_Calls_Repo()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Review { Id = 1 });

            await _service.DeleteReviewAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_Throws_When_Not_Found()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteReviewAsync(1));
        }
    }
}
