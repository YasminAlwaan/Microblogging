using Microsoft.AspNetCore.Mvc;
using Microblogging.Web.Controllers;
using Xunit;
using Microblogging.Data;
using Moq;

namespace Microblogging.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public void Login_ValidCredentials_RedirectsToPosts()
        {
            // Arrange
            var controller = new AccountController(
                Mock.Of<IConfiguration>(),
                Mock.Of<IRepository>());

            var model = new LoginModel
            {
                Username = "user1",
                Password = "password1"
            };

            // Act
            var result = controller.Login(model).Result;

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Posts", redirect.ControllerName);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var controller = new AccountController(
                Mock.Of<IConfiguration>(),
                Mock.Of<IRepository>());

            var model = new LoginModel
            {
                Username = "invalid",
                Password = "wrong"
            };

            // Act
            var result = controller.Login(model).Result;

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ModelState.ErrorCount > 0);
        }
    }
}