using FluentAssertions;
using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesJsonFileLoaderTests;

public class LoadAsyncTests
{
    [Fact]
    public async Task Should_Load_RecentBenches()
    {
        // Arrange

        var expectedLastOpened = DateTimeOffset.Now;
;
        var jsonFileLoaderMock = new Mock<IJsonFileLoader<RecentBenchesFileModel>>();
        var fileModel = new RecentBenchesFileModel
        {
            RecentBenches = new List<RecentBench>
            {
                new RecentBench
                {
                    BenchLabel = "benchLabel",
                    FilePath = "benchFilePath",
                    LastOpened = expectedLastOpened
                }
            }
        };

        jsonFileLoaderMock.Setup(l => l.LoadAsync(It.IsAny<string>())).ReturnsAsync(fileModel);

        var loader = new RecentBenchesJsonFileLoader(jsonFileLoaderMock.Object);

        // Act
        var result = await loader.LoadAsync("testPath");

        // Assert
        result.Should().HaveCount(1);
        result[0].BenchLabel.Should().Be("benchLabel");
        result[0].FilePath.Should().Be("benchFilePath");
        result[0].LastOpened.Should().Be(expectedLastOpened);
    }
}
