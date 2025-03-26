using Moq;
using Microblogging.Core;
using Microblogging.Data;
using Xunit;
using Microblogging.Core.Models;

namespace Microblogging.Tests.Services
{
    public class PostServiceTests
    {
        [Fact]
        public async Task CreatePost_ValidContent_CallsRepository()
        {
            // Arrange
            var mockRepo = new Mock<IRepository>();
            var service = new PostService(mockRepo.Object);
            var post = new Post { Content = "Valid content", UserId = 1 };

            // Act
            await service.CreatePost(post);

            // Assert
            mockRepo.Verify(r => r.CreatePost(It.IsAny<Post>()), Times.Once);
        }

        [Fact]
        public void ValidatePostContent_Exceeds140Chars_ThrowsException()
        {
            // Arrange
            var longContent = new string('a', 141);
            var post = new Post { Content = longContent };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => PostService.ValidatePostContent(post));
        }
    }
}