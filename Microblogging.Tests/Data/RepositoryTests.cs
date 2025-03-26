using Microsoft.EntityFrameworkCore;
using Microblogging.Core;
using Microblogging.Data;
using Xunit;
using Microblogging.Core.Models;

namespace Microblogging.Tests.Data
{
    public class RepositoryTests
    {
        private DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreatePost_SavesPostToDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var user = new User { Username = "testuser", PasswordHash = "hash" };

            using (var context = new AppDbContext(options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var repository = new Repository(context);
                var post = new Post
                {
                    Content = "Test post",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await repository.CreatePost(post);
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, await context.Posts.CountAsync());
                var savedPost = await context.Posts.FirstAsync();
                Assert.Equal("Test post", savedPost.Content);
            }
        }

        [Fact]
        public async Task GetTimeline_ReturnsPostsInCorrectOrder()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var user = new User { Username = "testuser", PasswordHash = "hash" };
            var posts = new[]
            {
                new Post { Content = "Post 1", CreatedAt = DateTime.UtcNow.AddHours(-1), User = user },
                new Post { Content = "Post 2", CreatedAt = DateTime.UtcNow, User = user }
            };

            using (var context = new AppDbContext(options))
            {
                context.Users.Add(user);
                context.Posts.AddRange(posts);
                await context.SaveChangesAsync();
            }

            // Act
            List<Post> result;
            using (var context = new AppDbContext(options))
            {
                var repository = new Repository(context);
                result = await repository.GetTimeline();
            }

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Post 2", result[0].Content);
            Assert.Equal("Post 1", result[1].Content);
        }
    }
}