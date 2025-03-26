using Microsoft.AspNetCore.Mvc;
using Moq;
using Microblogging.Web.Controllers;
using Microblogging.Core;
using Microblogging.Data;
using Xunit;
using Microblogging.Core.Models;
using Microblogging.Web.Services;

namespace Microblogging.Tests.Controllers
{
    public class PostsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithPosts()
        {
            // Arrange
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(r => r.GetTimeline())
                .ReturnsAsync(new List<Post> { new Post() });

            var controller = new PostsController(mockRepo.Object, Mock.Of<IImageService>());

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            // Arrange
            var controller = new PostsController(Mock.Of<IRepository>(), Mock.Of<IImageService>());

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new PostsController(Mock.Of<IRepository>(), Mock.Of<IImageService>());
            controller.ModelState.AddModelError("Content", "Required");

            var model = new CreatePostModel { Content = "" };

            // Act
            var result = await controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }
    }
}