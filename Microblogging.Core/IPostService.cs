using Microblogging.Core.Models;
using Microsoft.AspNetCore.Http;


namespace Microblogging.Core
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string content, int userId, IFormFile image = null);
        Task<IEnumerable<Post>> GetTimelineAsync();
        void ValidatePostContent(Post post);
    }
}
