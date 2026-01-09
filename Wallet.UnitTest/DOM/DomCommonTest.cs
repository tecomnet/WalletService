using System.Text;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Xunit;

namespace Wallet.UnitTest.DOM;

public class DomCommonTest
{
    [Fact]
    public void SafeParseConcurrencyToken_ShouldReturnBytes_WhenTokenIsValidBase64()
    {
        // Arrange
        var originalText = "TestToken";
        var validToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalText));

        // Act
        var result = DomCommon.SafeParseConcurrencyToken(validToken, "TestModule");

        // Assert
        Assert.Equal(Encoding.UTF8.GetBytes(originalText), result);
    }

    [Fact]
    public void SafeParseConcurrencyToken_ShouldThrowEmGeneralException_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "Invalid-Base64-String!";

        // Act & Assert
        var exception =
            Assert.Throws<EMGeneralAggregateException>(() =>
                DomCommon.SafeParseConcurrencyToken(invalidToken, "TestModule"));

        var emException = exception.InnerExceptions.First() as EMGeneralException;
        Assert.NotNull(emException);
        Assert.Equal(ServiceErrorsBuilder.ConcurrencyTokenInvalido, emException!.Code);
        Assert.Contains(invalidToken, emException.DescriptionDynamicContents.Select(x => x.ToString()));
        Assert.Equal("TestModule", emException.Module);
    }

    [Fact]
    public void SafeParseConcurrencyToken_ShouldThrowEmGeneralException_WhenTokenIsNull()
    {
        // Arrange
        string? invalidToken = null;

        // Act & Assert
        var exception =
            Assert.Throws<EMGeneralAggregateException>(() =>
                DomCommon.SafeParseConcurrencyToken(invalidToken!, "TestModule"));

        var emException = exception.InnerExceptions.First() as EMGeneralException;
        Assert.NotNull(emException);
        Assert.Equal(ServiceErrorsBuilder.ConcurrencyTokenRequerido, emException!.Code);
        Assert.Equal("TestModule", emException.Module);
    }

    [Fact]
    public void SafeParseConcurrencyToken_ShouldThrowEmGeneralException_WhenTokenIsEmpty()
    {
        // Arrange
        var invalidToken = string.Empty;

        // Act & Assert
        var exception =
            Assert.Throws<EMGeneralAggregateException>(() =>
                DomCommon.SafeParseConcurrencyToken(invalidToken, "TestModule"));

        var emException = exception.InnerExceptions.First() as EMGeneralException;
        Assert.NotNull(emException);
        Assert.Equal(ServiceErrorsBuilder.ConcurrencyTokenRequerido, emException!.Code);
        Assert.Equal("TestModule", emException.Module);
    }
}
