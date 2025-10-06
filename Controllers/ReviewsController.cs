using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Interfaces.ReviewInterfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _reviewService.GetReviewAsync(id);
        if (review == null) return NotFound();
        return Ok(review);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetReviewsByProductId(int productId)
    {
        var reviews = await _reviewService.GetReviewsByItemIdAsync(productId);
        if (reviews == null) return NotFound("This item has no reviews");
        return Ok(reviews);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        if (reviews == null) return NotFound("There are no reviews registered");
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto dto)
    {
        await _reviewService.CreateReviewAsync(dto);
        return Ok("Review created");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReviewStatus(int id, ReviewUpdateDto dto)
    {
        await _reviewService.UpdateReviewAsync(id, dto);
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return Ok("Deleted");
    }
}
