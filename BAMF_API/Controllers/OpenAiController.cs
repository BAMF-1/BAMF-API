namespace BAMF_API.Controllers
{
    using Azure.AI.OpenAI;
    using BAMF_API.Interfaces.ReviewInterfaces;
    using Microsoft.AspNetCore.Mvc;
    using OpenAI.Chat;
    using System.Text.Json;

    [ApiController]
    [Route("api/[controller]")]
    public class OpenAiController : ControllerBase
    {
        //private readonly AzureOpenAIClient _client;
        //private readonly string _deploymentName;
        private readonly IReviewService _reviewService;

        public OpenAiController(/*AzureOpenAIClient client, IConfiguration configuration,*/ IReviewService reviewService)
        {
            //_client = client;
            //_deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_NAME"] ?? "BAMF";
            _reviewService = reviewService;
        }

        // Updated endpoint - accepts slug instead of int productId
        [HttpGet("summarize/{slug}")]
        public async Task<IActionResult> SummarizeProductReviews(string slug)
        {
            // Get real reviews (still needed so endpoint behaves normally)
            var reviews = await _reviewService.GetReviewsByProductGroupSlugAsync(slug);

            if (reviews == null || !reviews.Any())
                return NotFound($"No reviews found for product group: {slug}");

            // TEMPORARY FAKE RESPONSE (REMOVE LATER)
            var fakeSummary = new
            {
                summary = $"This is a temporary fake summary for product group '{slug}'. " +
                          "Strengths: good build quality, solid performance. " +
                          "Weaknesses: slightly high price. Overall users are satisfied.",
                rating = 4,
                isFake = true
            };

            return Ok(fakeSummary);

            /*
            // ---------------- REAL API CALL (DISABLED TEMPORARILY) ----------------
            var reviewJson = JsonSerializer.Serialize(reviews);

            var messages = new List<MessageDto>
            {
                new MessageDto(
                    "system",
                    "Analyze the provided product reviews and generate a concise summary..."
                ),
                new MessageDto("user", reviewJson)
            };

            var chatClient = _client.GetChatClient(_deploymentName);

            var chatMessages = messages.Select<MessageDto, ChatMessage>(m => m.Role.ToLower() switch
            {
                "system" => new SystemChatMessage(m.Content),
                "user" => new UserChatMessage(m.Content),
                _ => new UserChatMessage(m.Content)
            }).ToList();

            var response = await chatClient.CompleteChatAsync(chatMessages);
            var reply = response.Value.Content[0].Text;

            return Ok(new { chatMessages, reply });
            */
        }


        public record ChatRequest(List<MessageDto> Messages);
        public record MessageDto(string Role, string Content);
    }
}