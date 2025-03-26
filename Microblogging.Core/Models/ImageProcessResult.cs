// Microblogging.Core/ImageProcessResult.cs (or Microblogging.Web/Models/ImageProcessResult.cs)
namespace Microblogging.Core.Models
{
    public class ImageProcessResult
    {
        public string OriginalUrl { get; }
        public Dictionary<string, string> ProcessedUrls { get; } // Key: dimension, Value: URL

        public ImageProcessResult(string originalUrl, Dictionary<string, string> processedUrls = null)

        {
            OriginalUrl = originalUrl;
            ProcessedUrls = processedUrls ?? new Dictionary<string, string>();
        }


        public void AddProcessedUrl(string dimension, string url)
        {
            ProcessedUrls[dimension] = url;
        }

        public string GetBestFitUrl(int width, int height)
        {
            if (!ProcessedUrls.Any())
                return OriginalUrl;

            // Simple algorithm to find the closest match
            var bestMatch = ProcessedUrls
                .OrderBy(p => Math.Abs(GetWidth(p.Key) - width) + Math.Abs(GetHeight(p.Key) - height))
                .First();

            return bestMatch.Value;
        }

        private int GetWidth(string dimension)
        {
            return int.Parse(dimension.Split('x')[0]);
        }

        private int GetHeight(string dimension)
        {
            return int.Parse(dimension.Split('x')[1]);
        }
    }
}