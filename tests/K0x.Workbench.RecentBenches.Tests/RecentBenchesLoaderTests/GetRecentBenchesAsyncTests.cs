using K0x.Workbench.RecentBenches;
using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using K0x.Workbench.RecentBenches.Internal;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesLoaderTests;

public class GetRecentBenchesAsyncTests
{
    [Fact]
    public async Task ReturnsRecentBenches_WhenFileExists()
    {
        // Arrange
        var mockJsonFileLoader = new Mock<IRecentBenchesJsonFileLoader>();
        var mockFilePathProvider = new Mock<IRecentBenchesFilePathProvider>();
        var expectedFilePath = "testFilePath.json";
        var expectedBenches = new List<RecentBench>
        {
            new RecentBench
            {
                BenchLabel = "Bench1",
                FilePath = "Path1",
                LastOpened = DateTimeOffset.UtcNow.AddDays(-1)
            },
            new RecentBench
            {
                BenchLabel = "Bench2",
                FilePath = "Path2",
                LastOpened = DateTimeOffset.UtcNow.AddDays(-2)
            }
        };

        mockFilePathProvider.Setup(p => p.GetFilePath()).Returns(expectedFilePath);
        mockJsonFileLoader.Setup(l => l.LoadAsync(expectedFilePath))
            .ReturnsAsync(expectedBenches);

        var loader = new RecentBenchesLoader(
            mockJsonFileLoader.Object,
            mockFilePathProvider.Object);

        // Act
        var result = await loader.GetRecentBenchesAsync();

        // Assert
        Assert.Equal(expectedBenches, result);
        mockFilePathProvider.Verify(p => p.GetFilePath(), Times.Once);
        mockJsonFileLoader.Verify(l => l.LoadAsync(expectedFilePath), Times.Once);
    }

    [Fact]
    public async Task ReturnsEmptyList_WhenFileDoesNotExist()
    {
        // Arrange
        var mockJsonFileLoader = new Mock<IRecentBenchesJsonFileLoader>();
        var mockFilePathProvider = new Mock<IRecentBenchesFilePathProvider>();
        var expectedFilePath = "nonExistentFile.json";

        mockFilePathProvider.Setup(p => p.GetFilePath()).Returns(expectedFilePath);
        mockJsonFileLoader.Setup(l => l.LoadAsync(expectedFilePath))
            .ReturnsAsync(new List<RecentBench>());

        var loader = new RecentBenchesLoader(mockJsonFileLoader.Object, mockFilePathProvider.Object);

        // Act
        var result = await loader.GetRecentBenchesAsync();

        // Assert
        Assert.Empty(result);
        mockFilePathProvider.Verify(p => p.GetFilePath(), Times.Once);
        mockJsonFileLoader.Verify(l => l.LoadAsync(expectedFilePath), Times.Once);
    }
}
