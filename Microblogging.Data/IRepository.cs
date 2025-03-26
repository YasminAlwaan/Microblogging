using Microblogging.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microblogging.Data
{
    public interface IRepository
    {
        Task CreatePost(Post post);
        Task<List<Post>> GetTimeline();
        Task<User> GetUser(string username);
    }
}
