using Hangfire;
using Microblogging.Core.Interfaces;
using Microblogging.Core.Models;

namespace Microblogging.Web.Services
{
    public interface IImageService
    {
        Task<ImageProcessResult> ProcessImage(IFormFile file);
    }

    public class ImageService : IImageService
    {
        private readonly IImageStorage _storage;
        private readonly IWebHostEnvironment _env;

        public ImageService(IImageStorage storage, IWebHostEnvironment env)
        {
            _storage = storage;
            _env = env;
        }

        public async Task<ImageProcessResult> ProcessImage(IFormFile file)
        {
            // Validation
            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("File size exceeds 2MB");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(extension))
                throw new InvalidOperationException("Invalid file format");

            // Store original
            var originalUrl = await _storage.StoreImage(file.OpenReadStream(),
                $"original_{Guid.NewGuid()}{extension}");

            // Start background job for processing
            BackgroundJob.Enqueue<IImageProcessor>(p =>
                p.ProcessImage(originalUrl, file.ContentType));

            return new ImageProcessResult(originalUrl);
        }
    }
}
