using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LittleUrl.Website.Controllers;
using LittleUrl.Website.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using LittleUrl.Domain.Repositories;
using LittleUrl.Infrastructure;
using LittleUrl.Domain;
using System.Linq.Expressions;
using System;
using FluentAssertions.Equivalency;

namespace LittleUrl.API.Tests.Controllers
{
    public class LitlUrlServiceTest
    {
        private readonly Mock<ILitlUrlRepository> litlUrlRepository;
        private readonly Mock<ILogger<LitlUrlService>> logger;

        public LitlUrlServiceTest()
        {
            litlUrlRepository = new Mock<ILitlUrlRepository>();
            logger = new Mock<ILogger<LitlUrlService>>();
        }

        #region "Test AddLitlUrl"`

        [Fact]
        public void AddLitlUrl_ErrorOccursOnGetAsync_ReturnsNullAndLogsError()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>> ()))
                .ThrowsAsync(new Exception());

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().BeNull();
            logger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error occured." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectExists_ReturnsCode()
        {
            string code = "1234";
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(new LitlUrl(code, It.IsAny<string>()));

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().NotBeNull();
            response.Should().Be(code);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectNotExists_ErrorOnGetExistsAsync_ReturnsNullAndLogsError()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            litlUrlRepository
                .Setup(m => m.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .Throws(new Exception());

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);  
            
            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().BeNull();
            litlUrlRepository.Verify(l => l.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()), Times.Once);
            logger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error occured." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectNotExists_GenerateCodeFirstTime_ErrorOnAddToRepository_ReturnsNullAndLogsError()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            litlUrlRepository
                .Setup(m => m.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(false);

            litlUrlRepository
                .Setup(m => m.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().BeNull();
            litlUrlRepository.Verify(l => l.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()), Times.Once);
            litlUrlRepository.Verify(l => l.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            logger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error occured." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectNotExists_GenerateCodeFirstTime_AddToRepository_ErrorOnSave_ReturnsNullAndLogsError()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            litlUrlRepository
                .Setup(m => m.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(false);

            litlUrlRepository
                .Setup(m => m.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new LitlUrl(It.IsAny<string>(), It.IsAny<string>()));

            litlUrlRepository
                .Setup(m => m.Save())
                .Throws(new Exception());

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().BeNull();
            litlUrlRepository.Verify(l => l.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()), Times.Once);
            litlUrlRepository.Verify(l => l.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            litlUrlRepository.Verify(l => l.Save(), Times.Once);
            logger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error occured." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectNotExists_GenerateCodeFirstTime_AddToRepository_Saves_ReturnsCode()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            litlUrlRepository
                .Setup(m => m.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(false);

            string code = "1234";
            litlUrlRepository
                .Setup(m => m.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new LitlUrl(code, It.IsAny<string>()));

            litlUrlRepository
                .Setup(m => m.Save())
                .ReturnsAsync(1);

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().NotBeNull();
            response.Should().BeOfType<string>();
            response.Should().Be(code);
            litlUrlRepository.Verify(l => l.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()), Times.Once);
            litlUrlRepository.Verify(l => l.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            litlUrlRepository.Verify(l => l.Save(), Times.Once);
        }

        [Fact]
        public void AddLitlUrl_GetAsync_ObjectNotExists_GenerateCodeSecondTime_AddToRepository_Saves_ReturnsCode()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            litlUrlRepository
                .SetupSequence(m => m.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            string code = "1234";
            litlUrlRepository
                .Setup(m => m.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new LitlUrl(code, It.IsAny<string>()));

            litlUrlRepository
                .Setup(m => m.Save())
                .ReturnsAsync(1);

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.AddLitlUrl("https://www.google.com").Result;

            response.Should().NotBeNull();
            response.Should().BeOfType<string>();
            response.Should().Be(code);
            litlUrlRepository.Verify(l => l.GetExistsAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()), Times.Exactly(2));
            litlUrlRepository.Verify(l => l.AddLitlUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            litlUrlRepository.Verify(l => l.Save(), Times.Once);
        }

        #endregion

        #region "Test GetWithCode"

        [Fact]
        public void GetWithCode_ErrorOccursOnGetAsync_ReturnsNullAndLogsError()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ThrowsAsync(new Exception());

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.GetWithCode("1234").Result;

            response.Should().BeNull();
            logger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Error occured." && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetWithCode_GetAsync_NoObjectFound_ReturnsEmptyString()
        {
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(value: null);

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.GetWithCode("1234").Result;

            response.Should().NotBeNull();
            response.Should().BeOfType<string>();
            response.Should().Be(string.Empty);
        }

        [Fact]
        public void GetWithCode_GetAsync_NoObjectFound_ReturnsLongUrl()
        {
            string longUrl = "https://www.google.com";
            litlUrlRepository
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<LitlUrl, bool>>>()))
                .ReturnsAsync(new LitlUrl(It.IsAny<string>(), longUrl));

            var service = new LitlUrlService(litlUrlRepository.Object, logger.Object);

            var response = service.GetWithCode("1234").Result;

            response.Should().NotBeNull();
            response.Should().BeOfType<string>();
            response.Should().Be(longUrl);
        }

        #endregion
    }
}
