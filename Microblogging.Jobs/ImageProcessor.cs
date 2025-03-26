using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Microblogging.Core.Interfaces;

namespace Microblogging.Jobs;



public class ImageProcessor : IImageProcessor
{
    private readonly IImageStorage _storage;
    private readonly HttpClient _httpClient;

    public ImageProcessor(IImageStorage storage, HttpClient httpClient)
    {
        _storage = storage;
        _httpClient = httpClient;
    }

    public async Task ProcessImage(string imageUrl, string contentType)
    {
        try
        {
            // Download the image from the URL
            using var response = await _httpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();

            using var imageStream = await response.Content.ReadAsStreamAsync();

            // Load the image
            using var image = await Image.LoadAsync(imageStream);

            // Define common dimensions
            var sizes = new[] { (800, 600), (1024, 768), (400, 300) };

            foreach (var (width, height) in sizes)
            {
                // Clone and resize the image
                var resizedImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Max
                }));

                // Convert to WebP format
                using var outputStream = new MemoryStream();
                await resizedImage.SaveAsync(outputStream, new WebpEncoder());
                outputStream.Position = 0;

                // Store the processed image
                var fileName = $"{Path.GetFileNameWithoutExtension(imageUrl)}_{width}x{height}.webp";
                await _storage.StoreImage(outputStream, fileName);
            }
        }
        catch (Exception ex)
        {
            // Handle or log the error
            Console.WriteLine($"Error processing image: {ex.Message}");
            throw;
        }
    }
}
