using Moq;
using Microblogging.Web.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Microblogging.Tests.Services
{
    public class ImageServiceTests
    {
        [Fact]
        public async Task ProcessImage_ValidFile_ReturnsUrl()
        {
            // Arrange
            var mockStorage = new Mock<IImageStorage>();
            mockStorage.Setup(s => s.StoreImage(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://storage.test/image.jpg");

            var service = new ImageService(mockStorage.Object, Mock.Of<IWebHostEnvironment>());
            var file = CreateTestFile("test.jpg", "image/jpeg");

            // Act
            var result = await service.ProcessImage(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://storage.test/image.jpg", result.OriginalUrl);
        }

        [Fact]
        public async Task ProcessImage_InvalidFileType_ThrowsException()
        {
            // Arrange
            var service = new ImageService(Mock.Of<IImageStorage>(), Mock.Of<IWebHostEnvironment>());
            var file = CreateTestFile("test.txt", "text/plain");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ProcessImage(file));
        }

        private IFormFile CreateTestFile(string fileName, string contentType)
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream(new byte[1024]);

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            return fileMock.Object;
        }
    }
}