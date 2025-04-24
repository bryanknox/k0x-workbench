using FluentAssertions;
using K0x.Workbench.Files.Abstractions;
using K0x.Workbench.RecentBenches.Internal;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesFilePathProviderTests;

public class GetFilePathTests
{
    private readonly Mock<ILocalAppDataFolderPathProvider> _localAppDataFolderPathProviderMock;
    private readonly RecentBenchesFilePathProvider _provider;

    public GetFilePathTests()
    {
        _localAppDataFolderPathProviderMock = new Mock<ILocalAppDataFolderPathProvider>();
        _provider = new RecentBenchesFilePathProvider(_localAppDataFolderPathProviderMock.Object);
    }

    [Fact]
    public void Should_Return_FilePath()
    {
        // Arrange
        _localAppDataFolderPathProviderMock.Setup(p => p.GetLocalAppDataFolderPath())
            .Returns("defaultFolder");

        // Act
        var result = _provider.GetFilePath();

        // Assert
        result.Should().Be($"defaultFolder\\{RecentBenchesConstants.DefaultFileName}");
    }
}
