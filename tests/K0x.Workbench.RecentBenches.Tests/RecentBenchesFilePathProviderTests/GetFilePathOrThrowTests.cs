using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesFilePathProviderTests;

public class GetFilePathOrThrowTests
{
    [Fact]
    public void Should_Return_FilePath()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["RecentBenchesFilePath"]).Returns("testPath");

        var provider = new RecentBenchesFilePathProvider(configurationMock.Object);

        // Act
        var result = provider.GetFilePathOrThrow();

        // Assert
        result.Should().Be("testPath");
    }

    [Fact]
    public void Should_Throw_Exception_When_FilePath_Is_Not_Set()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["RecentBenchesFilePath"]).Returns((string)null!);

        var provider = new RecentBenchesFilePathProvider(configurationMock.Object);

        // Act
        Action act = () => provider.GetFilePathOrThrow();

        // Assert
        act.Should()
            .Throw<RecentBenchesFileException>()
            .WithMessage("RecentBenchesFilePath is not set in appsettings.json.");
    }
}
