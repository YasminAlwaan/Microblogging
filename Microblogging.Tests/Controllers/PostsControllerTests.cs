using Microsoft.AspNetCore.Mvc;
using Moq;
using Microblogging.Core;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microblogging.Core.Models;
using Microblogging.Web.Controllers;
using FluentAssertions;

namespace Microblogging.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IPostService> _mockPostService;
        private readonly Mock<ILogger<PostsController>> _mockLogger;
        private readonly PostsController _controller;

        public PostsControllerTests()
        {
            _mockPostService = new Mock<IPostService>();
            _mockLogger = new Mock<ILogger<PostsController>>();
            _controller = new PostsController(_mockPostService.Object, _mockLogger.Object);

            // Setup controller context with JWT claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsOkWithPosts_WhenServiceSucceeds()
        {
            // Arrange
            var testPosts = new List<Post>
            {
                new Post { Id = 1, Content = "Test post 1", User = new User { Username = "user1" } },
                new Post { Id = 2, Content = "Test post 2", User = new User { Username = "user2" } }
            };

            _mockPostService.Setup(x => x.GetTimelineAsync())
                .ReturnsAsync(testPosts);

            // Act
            var result = await _controller.Index();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var posts = okResult.Value.Should().BeAssignableTo<IEnumerable<object>>().Subject;
            posts.Should().HaveCount(2);
        }

        [Fact]
        public async Task Index_ReturnsServerError_WhenServiceThrowsException()
        {
            // Arrange
            _mockPostService.Setup(x => x.GetTimelineAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void GetOptimalImageUrl_ReturnsOriginal_WhenNoProcessedUrl()
        {
            // Arrange
            var post = new Post { OriginalImageUrl = "original.jpg" };
            var controller = new PostsController(_mockPostService.Object, _mockLogger.Object);

            // Act
            var result = controller.GetOptimalImageUrl(post, 800, 600);

            // Assert
            result.Should().Be("original.jpg");
        }

        [Fact]
        public void GetOptimalImageUrl_ReturnsProcessed_WhenAvailable()
        {
            // Arrange
            var post = new Post
            {
                OriginalImageUrl = "original.jpg",
                ProcessedImageUrl = "processed.webp"
            };
            var controller = new PostsController(_mockPostService.Object, _mockLogger.Object);

            // Act
            var result = controller.GetOptimalImageUrl(post, 800, 600);

            // Assert
            result.Should().Be("processed.webp");
        }

        [Fact]
        public void GetHeaderValue_ReturnsDefault_WhenHeaderMissing()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = _controller.GetHeaderValue("X-Missing-Header", 1024);

            // Assert
            result.Should().Be(1024);
        }

        [Fact]
        public void GetHeaderValue_ReturnsParsedValue_WhenHeaderExists()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Test-Header"] = "768";
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = _controller.GetHeaderValue("X-Test-Header", 1024);

            // Assert
            result.Should().Be(768);
        }

        [Fact]
        public void GetOptimalImageUrl_ReturnsBestMatch_WhenMultipleProcessedUrlsExist()
        {
            // Arrange
            var post = new Post
            {
                OriginalImageUrl = "original.jpg",
                ProcessedImageUrls = new Dictionary<string, string>
        {
            { "800x600", "processed800.webp" },
            { "1024x768", "processed1024.webp" },
            { "400x300", "processed400.webp" }
        }
            };

            var controller = new PostsController(_mockPostService.Object, _mockLogger.Object);

            // Act - Test with screen size closest to 800x600
            var result = controller.GetOptimalImageUrl(post, 850, 650);

            // Assert
            result.Should().Be("processed800.webp");
        }
    }
}