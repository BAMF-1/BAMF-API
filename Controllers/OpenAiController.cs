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
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;
        private readonly IReviewService _reviewService;

        public OpenAiController(AzureOpenAIClient client, IConfiguration configuration, IReviewService reviewService)
        {
            _client = client;
            _deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_NAME"] ?? "BAMF";
            _reviewService = reviewService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var messages = req.Messages.Select<MessageDto, ChatMessage>(m => m.Role.ToLower() switch
            {
                "system" => new SystemChatMessage(m.Content),
                "user" => new UserChatMessage(m.Content),
                "assistant" => new AssistantChatMessage(m.Content),
                _ => new UserChatMessage(m.Content)
            }).ToList();

            var response = await chatClient.CompleteChatAsync(messages);
            var reply = response.Value.Content[0].Text;
            return Ok(new { reply });
        }

        // Updated endpoint - accepts slug instead of int productId
        [HttpGet("summarize/{slug}")]
        public async Task<IActionResult> SummarizeProductReviews(string slug)
        {
            var reviews = await _reviewService.GetReviewsByProductGroupSlugAsync(slug);

            if (reviews == null || !reviews.Any())
                return NotFound($"No reviews found for product group: {slug}");

            var reviewJson = JsonSerializer.Serialize(reviews);

            var messages = new List<MessageDto>
            {
                new MessageDto(
                    "system",
                    "Summarize user-provided product reviews and rate the product 1–5 stars based on overall sentiment, credibility, and review trends over time."
                ),
                new MessageDto(
                    "user",
                    reviewJson
                )
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
            //return Ok(new { summary = reply });
        }
    }

    public record ChatRequest(List<MessageDto> Messages);
    public record MessageDto(string Role, string Content);
}