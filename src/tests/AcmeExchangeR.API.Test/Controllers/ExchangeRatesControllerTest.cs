using AcmeExchangeR.API.Controllers;
using AcmeExchangeR.Bus.Services.Abstraction;
using AcmeExchangeR.Data.Entities;
using AcmeExchangeR.Utils.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AcmeExchangeR.API.Test.Controllers;

public class ExchangeRatesControllerTest
{
    private Mock<IRateService> _mockRateService;
    private Mock<IMemoryCache> _mockMemoryCache;
    private ExchangeRatesController _controller;
    private Mock<ILogger<ExchangeRatesController>> _mockLogger;
    private Mock<IConfiguration> _mockConfiguration; 


    public ExchangeRatesControllerTest()
    {
        _mockRateService = new Mock<IRateService>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<ExchangeRatesController>>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task Get_Should_Return_OK_when_Everything_is_working()
    { 
        //Arrange
        _mockRateService.Setup(x => x.GetByCurrencyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRate
            {
                Id = 1,
                Payload = new FastForexResponse
                {
                    Base = "USD",
                    Updated = DateTime.Now.ToLongDateString()
                }
            });

        _mockConfiguration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);  
        _mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns(false);
        
        // _mockMemoryCache.Setup(x => x.Set(It.IsAny<object>(), It.IsAny<object>()))
        //     .Returns(true);
        
        // _mockLogger.Setup(x => x.LogInformation(It.IsAny<string>()));
        // _mockLogger.Setup(x => x.LogWarning(It.IsAny<string>()));

        _controller = new ExchangeRatesController(_mockRateService.Object,
            _mockMemoryCache.Object, 
            _mockLogger.Object,
            _mockConfiguration.Object);

        //Act
        var response = await _controller.GetExchangeRate("USD", CancellationToken.None);
        var okResult = response as OkObjectResult;

        //Assert
        Assert.NotNull(okResult);
        
        Assert.Equal(200, okResult.StatusCode);
    }
}