using Microblogging.Core.Models;
using Microblogging.Data;
using Microblogging.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Microblogging.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IImageService _imageService;

        public PostsController(IRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _repository.GetTimeline();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Content,Image")] CreatePostModel model)
        {
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    Content = model.Content,
                    CreatedAt = DateTime.UtcNow,
                    Latitude = GenerateRandomCoordinate(),
                    Longitude = GenerateRandomCoordinate(),
                    UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
                };

                if (model.Image != null)
                {
                    var result = await _imageService.ProcessImage(model.Image);
                    post.OriginalImageUrl = result.OriginalUrl;
                }

                await _repository.CreatePost(post);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private double GenerateRandomCoordinate()
        {
            var random = new Random();
            return random.NextDouble() * 180 - 90;
        }
    }
}
