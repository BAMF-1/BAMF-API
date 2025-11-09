using BAMF_API.DTOs.Requests.ReviewDTOs;
using BAMF_API.Interfaces.ReviewInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,User")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Everyone can see a specific review
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _reviewService.GetReviewAsync(id);
        if (review == null) return NotFound();
        return Ok(review);
    }

    [HttpGet("product/{productId}")]
    [AllowAnonymous] // Everyone can see reviews for a product
    public async Task<IActionResult> GetReviewsByProductId(int productId)
    {
        var reviews = await _reviewService.GetReviewsByItemIdAsync(productId);
        if (reviews == null) return NotFound("This item has no reviews");
        return Ok(reviews);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] // Only Admins can see all reviews (Moderation)
    public async Task<IActionResult> GetAllReviews(int page)
    {
        var reviews = await _reviewService.GetAllReviewsAsync(page);
        if (reviews == null) return NotFound("There are no reviews registered");
        return Ok(reviews);
    }

    [HttpGet("count")]
    [Authorize(Roles = "Admin")] // Only Admins can see review count of all items
    public async Task<IActionResult> GetReviewCount()
    {
        var count = await _reviewService.GetReviewsCountAsync();
        return Ok(new { count });
    }

    [HttpPost]
    // Only authenticated users can create reviews
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto dto)
    {
        await _reviewService.CreateReviewAsync(dto);
        return Ok("Review created");
    }

    [HttpPut("{id}")]
    // Only Authenticated users can update reviews (Moderation and revisions)
    public async Task<IActionResult> UpdateReview(int id, ReviewUpdateDto dto)
    {
        await _reviewService.UpdateReviewAsync(id, dto);
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    // Only Authenticated users can update reviews (Moderation and revisions)
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return Ok("Deleted");
    }
}
