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
    public void Should_Return_FilePath()
    {
        // Arrange
        _appExeFolderPathProviderMock.Setup(p => p.GetAppExeFolderPath())
            .Returns("defaultFolder");

        // Act
        var result = _provider.GetFilePath();

        // Assert
        result.Should().Be($"defaultFolder\\{RecentBenchesConstants.DefaultFileName}");
    }
}
