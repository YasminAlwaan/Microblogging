using Moq;
using Microblogging.Core;
using Microblogging.Data;
using Xunit;
using Microblogging.Core.Models;
using Microblogging.Web.Services;

namespace Microblogging.Tests.Services
{
    public class PostServiceTests
    {
        [Fact]
        public async Task CreatePostAsync_ValidContent_CallsRepository()
        {
            // Arrange
            var mockRepo = new Mock<IRepository>();
            var mockImageService = new Mock<IImageService>();
            var service = new PostService(mockRepo.Object, mockImageService.Object);

            var testContent = "Valid post content";
            var testUserId = 1;

            // Act
            await service.CreatePostAsync(testContent, testUserId);

            // Assert
            mockRepo.Verify(r => r.CreatePost(It.IsAny<Post>()), Times.Once);
        }

        [Fact]
        public void ValidatePostContent_Exceeds140Chars_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IRepository>();
            var mockImageService = new Mock<IImageService>();
            var service = new PostService(mockRepo.Object, mockImageService.Object);

            var longContent = new string('a', 141);
            var post = new Post { Content = longContent };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ValidatePostContent(post));
        }

    }
}