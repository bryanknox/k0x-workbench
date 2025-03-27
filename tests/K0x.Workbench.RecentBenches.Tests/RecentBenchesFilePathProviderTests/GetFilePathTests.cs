using FluentAssertions;
using K0x.Workbench.RecentBenches.Internal;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesFilePathProviderTests;

public class GetFilePathTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IAppExeFolderPathProvider> _appExeFolderPathProviderMock;
    private readonly RecentBenchesFilePathProvider _provider;

    public GetFilePathTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _appExeFolderPathProviderMock = new Mock<IAppExeFolderPathProvider>();
        _provider = new RecentBenchesFilePathProvider(_appExeFolderPathProviderMock.Object, _configurationMock.Object);
    }

    [Fact]
    public void Should_Return_Configured_FilePath()
    {
        // Arrange
        _configurationMock.Setup(c => c[RecentBenchesConstants.FilePathSettingName])
            .Returns("testPath");

        // Act
        var result = _provider.GetFilePath();

        // Assert
        result.Should().Be("testPath");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")] // whitespace
    public void Should_Return_Default_FilePath_When_Configured_FilePath_Is_Invalid(
        string? configuredFilePath)
    {
        // Arrange
        _configurationMock.Setup(c => c[RecentBenchesConstants.FilePathSettingName])
            .Returns(configuredFilePath);
        _appExeFolderPathProviderMock.Setup(p => p.GetAppExeFolderPath())
            .Returns("defaultFolder");

        // Act
        var result = _provider.GetFilePath();

        // Assert
        result.Should().Be($"defaultFolder\\{RecentBenchesConstants.DefaultFileName}");
    }
}
