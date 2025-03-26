using static System.Net.Mime.MediaTypeNames;

namespace Microblogging.Jobs;

public interface IImageProcessor
{
    Task ProcessImage(string imageUrl, string contentType);
}

public class ImageProcessor : IImageProcessor
{
    private readonly IImageStorage _storage;

    public ImageProcessor(IImageStorage storage)
    {
        _storage = storage;
    }

    public async Task ProcessImage(string imageUrl, string contentType)
    {
        using var image = await Image.LoadAsync(imageUrl);

        // Resize to common dimensions
        var sizes = new[] { (800, 600), (1024, 768), (400, 300) };

        foreach (var (width, height) in sizes)
        {
            var clone = image.Clone(ctx => ctx.Resize(width, height));
            await _storage.StoreImage(
                clone.AsWebp(),
                $"{Path.GetFileNameWithoutExtension(imageUrl)}_{width}x{height}.webp");
        }
    }
}