using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using Moq;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesJsonFileSaverTests;

public class SaveAsyncTests
{
    [Fact]
    public async Task Should_Save_RecentBenches()
    {
        // Arrange

        var expectedLastOpened = DateTimeOffset.Now;

        var jsonFileSaverMock = new Mock<IJsonFileSaver<RecentBenchesFileModel>>();
        var saver = new RecentBenchesJsonFileSaver(jsonFileSaverMock.Object);

        var recentBenches = new List<RecentBench>
        {
            new RecentBench
            {
                FilePath = "benchFilePath",
                BenchLabel = "benchLabel",
                LastOpened = expectedLastOpened
            }
        };

        // Act
        await saver.SaveAsync(recentBenches, "testPath");

        // Assert
        jsonFileSaverMock.Verify(
            s => s.SaveAsync(
                It.Is<RecentBenchesFileModel>(
                    f => f.RecentBenches.Count == 1
                    && f.RecentBenches[0].BenchLabel == "benchLabel"
                    && f.RecentBenches[0].FilePath == "benchFilePath"
                    && f.RecentBenches[0].LastOpened == expectedLastOpened
                 ),
                "testPath"),
            Times.Once);
    }
}
