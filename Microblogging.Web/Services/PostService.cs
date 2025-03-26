using Microblogging.Core;
using Microblogging.Core.Models;
using Microblogging.Data;

namespace Microblogging.Web.Services
{

    public class PostService : IPostService
    {
        private readonly IRepository _repository;
        private readonly IImageService _imageService;

        public PostService(IRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        public async Task<Post> CreatePostAsync(string content, int userId, IFormFile image = null)
        {
            var post = new Post
            {
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Latitude = GenerateRandomCoordinate(),
                Longitude = GenerateRandomCoordinate()
            };

            ValidatePostContent(post);

            if (image != null)
            {
                var result = await _imageService.ProcessImage(image);
                post.OriginalImageUrl = result.OriginalUrl;
                post.ProcessedImageUrls = result.ProcessedUrls;
            }

            await _repository.CreatePost(post);
            return post;
        }

        public async Task<IEnumerable<Post>> GetTimelineAsync()
        {
            return await _repository.GetTimeline();
        }

        public void ValidatePostContent(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Content))
                throw new ArgumentException("Post content cannot be empty");

            if (post.Content.Length > 140)
                throw new ArgumentException("Post cannot exceed 140 characters");
        }

        private double GenerateRandomCoordinate()
        {
            var random = new Random();
            return random.NextDouble() * 180 - 90; // Range -90 to 90
        }
    }
}
