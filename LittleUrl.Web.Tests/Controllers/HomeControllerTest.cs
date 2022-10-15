using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LittleUrl.Website.Controllers;
using LittleUrl.Website.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using FluentAssertions.Equivalency;

namespace LittleUrl.API.Tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly Mock<ILitlUrlService> litlUrlService;
        private readonly Mock<ILogger<HomeController>> logger;

        public HomeControllerTest()
        {
            litlUrlService = new Mock<ILitlUrlService>();
            logger = new Mock<ILogger<HomeController>>();
        }

        #region "Test Post"

        [Theory]
        [InlineData("")]
        [InlineData("sasdsdfsadfdsf")]
        [InlineData(null)]
        public void Post_WithInvalidLongUrl_ReturnsBadRequest(string? longUrl)
        {
            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Post(longUrl).Result;

            response.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Post_WithValidUrl_ErrorOccurs_Returns500Error()
        {
            string? codeToRepond = null;
            litlUrlService
                .Setup(m => m.AddLitlUrl(It.IsAny<string>()))
                .ReturnsAsync(codeToRepond);

            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Post("https://www.google.com").Result;

            (response.Result as ObjectResult).StatusCode.Should().Be(500);
        }

        [Fact]
        public void Post_WithValidUrl_ReturnsCode()
        {
            string codeToRespond = "sdfsdf";
            litlUrlService
                .Setup(m => m.AddLitlUrl(It.IsAny<string>()))
                .ReturnsAsync(codeToRespond);

            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Post("https://www.google.com").Result;

            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(codeToRespond);
        }

        #endregion

        #region "Test Get"

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Get_WithNullOrEmptyCode_ReturnsBadRequest(string? code)
        {
            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Get(code);

            response.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Get_WithCode_ErrorOccurs_Returns500Error()
        {
            string? longUrl = null;
            litlUrlService
                .Setup(m => m.GetWithCode(It.IsAny<string>()))
                .ReturnsAsync(longUrl);

            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Get("1234");

            (response.Result as ObjectResult)?.StatusCode.Should().Be(500);
        }

        [Fact]
        public void Get_WithCode_NoLitlUrlFound_ReturnsNotFoundObject()
        {
            string? longUrl = string.Empty;
            litlUrlService
                .Setup(m => m.GetWithCode(It.IsAny<string>()))
                .ReturnsAsync(longUrl);

            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Get("1234");

            response.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Get_WithCode_LitlUrlFound_ReturnsLongUrl()
        {
            string? longUrl = "https://www.google.com";
            litlUrlService
                .Setup(m => m.GetWithCode(It.IsAny<string>()))
                .ReturnsAsync(longUrl);

            var controller = new HomeController(litlUrlService.Object, logger.Object);

            var response = controller.Get("1234");

            ((RedirectResult)response.Result).Url.Should().Be(longUrl);
        }

        #endregion
    }
}
