using Microblogging.Core;
using Microblogging.Core.Models;
using Microblogging.Data;
using Microblogging.Web.Models;
using Microblogging.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Microblogging.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {

                var posts = await _postService.GetTimelineAsync();

                // Get client dimensions from headers 
                var screenWidth = GetHeaderValue("X-Screen-Width", 800);
                var screenHeight = GetHeaderValue("X-Screen-Height", 600);

                var response = posts.Select(p => new
                {
                    Id = p.Id,
                    Content = p.Content,
                    AuthorName = p.User?.Username ?? "Unknown",
                    CreatedAt = p.CreatedAt.ToString("g"),
                    ImageUrl = GetOptimalImageUrl(p, screenWidth, screenHeight),
                    Location = $"{p.Latitude:F2}, {p.Longitude:F2}"
                });
                ViewBag.JwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving posts");
                return StatusCode(500, new { Message = "An error occurred while loading posts." });
            }
        }

        internal int GetHeaderValue(string header, int defaultValue)
        {
            if (Request.Headers.TryGetValue(header, out var value) &&
                int.TryParse(value, out var intValue))
            {
                return intValue;
            }
            return defaultValue;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _postService.CreatePostAsync(model.Content, userId, model.Image);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        internal double GenerateRandomCoordinate()
        {
            var random = new Random();
            return random.NextDouble() * 180 - 90;
        }
        internal string GetOptimalImageUrl(Post post, int screenWidth, int screenHeight)
        {
            if (string.IsNullOrEmpty(post.OriginalImageUrl))
                return null;

            // If no processed images, return original
            if (post.ProcessedImageUrls == null || !post.ProcessedImageUrls.Any())
                return post.OriginalImageUrl;

            // Find the closest matching resolution
            var bestMatch = post.ProcessedImageUrls
                .Select(kvp => new {
                    Dimensions = kvp.Key,
                    Url = kvp.Value,
                    Width = int.Parse(kvp.Key.Split('x')[0]),
                    Height = int.Parse(kvp.Key.Split('x')[1])
                })
                .OrderBy(img => Math.Abs(img.Width - screenWidth) +
                                Math.Abs(img.Height - screenHeight))
                .FirstOrDefault();

            return bestMatch?.Url ?? post.OriginalImageUrl;
        }
    }
}
