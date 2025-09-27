using FluentAssertions;
using K0x.Workbench.Files.Abstractions;
using K0x.Workbench.RecentBenches.Internal;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesFilePathProviderTests;

public class GetFilePathTests
{
    private readonly Mock<IDataFolderPathProvider> _dataFolderPathProviderMock;
    private readonly RecentBenchesFilePathProvider _provider;

    public GetFilePathTests()
    {
        _dataFolderPathProviderMock = new Mock<IDataFolderPathProvider>();
        _provider = new RecentBenchesFilePathProvider(_dataFolderPathProviderMock.Object);
    }

    [Fact]
    public void Should_Return_FilePath()
    {
        // Arrange
        _dataFolderPathProviderMock.Setup(p => p.GetDataFolderPath())
            .Returns("defaultFolder");

        // Act
        var result = _provider.GetFilePath();

        // Assert
        result.Should().Be($"defaultFolder\\{RecentBenchesConstants.DefaultFileName}");
    }
}
