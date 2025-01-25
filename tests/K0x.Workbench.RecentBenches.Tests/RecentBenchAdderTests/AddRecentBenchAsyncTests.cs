using K0x.Workbench.RecentBenches.Abstractions;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using Moq;
using Xunit;
using Microsoft.Extensions.Time.Testing;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchAdderTests;

public class AddRecentBenchAsyncTests
{
    [Fact]
    public async Task Should_Add_RecentBench()
    {
        // Arrange

        var filePathProviderMock = new Mock<IRecentBenchesFilePathProvider>();
        var fileLoaderMock = new Mock<IRecentBenchesJsonFileLoader>();
        var fileSaverMock = new Mock<IRecentBenchesJsonFileSaver>();
        var fakeTimeProvider = new FakeTimeProvider();

        filePathProviderMock.Setup(p => p.GetFilePathOrThrow())
            .Returns("testPath");
        fileLoaderMock.Setup(l => l.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<RecentBench>());
        fakeTimeProvider.Advance(TimeSpan.FromHours(1));

        var adder = new RecentBenchAdder(
            filePathProviderMock.Object,
            fileLoaderMock.Object,
            fileSaverMock.Object,
            fakeTimeProvider
        );

        // Act
        await adder.AddRecentBenchAsync("benchFilePath", "benchLabel");

        // Assert
        var foo = fakeTimeProvider.GetLocalNow();

        fileSaverMock.Verify(s => s.SaveAsync(
            It.Is<List<RecentBench>>(b => b.Count == 1
                && b[0].FilePath == "benchFilePath"
                && b[0].BenchLabel == "benchLabel"
                && b[0].LastOpened == fakeTimeProvider.GetLocalNow()),
            "testPath"),
            Times.Once
        );
    }
}
