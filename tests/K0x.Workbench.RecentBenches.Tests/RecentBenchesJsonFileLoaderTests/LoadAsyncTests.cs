using FluentAssertions;
using K0x.DataStorage.JsonFiles;
using K0x.Workbench.RecentBenches.Abstractions.Models;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace K0x.Workbench.RecentBenches.Tests.RecentBenchesJsonFileLoaderTests;

public class LoadAsyncTests
{
    [Fact]
    public async Task Should_Return_Recent_When_File_Exists()
    {
        // Arrange
        var expectedLastOpened = DateTimeOffset.Now;
        const string expectedBenchLabel = "benchLabel";
        const string testBenchFilePath = "C:/foo/bar.json";

        var jsonFileLoaderMock = new Mock<IJsonFileLoader<RecentBenchesFileModel>>();
        var fileModel = new RecentBenchesFileModel
        {
            RecentBenches = new List<RecentBench>
            {
                new RecentBench
                {
                    BenchLabel = expectedBenchLabel,
                    FilePath = testBenchFilePath,
                    LastOpened = expectedLastOpened
                }
            }
        };

        jsonFileLoaderMock.Setup(l => l.LoadAsync(It.IsAny<string>())).ReturnsAsync(fileModel);

        var fileSystemMock = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                        {
                            testBenchFilePath,
                            new MockFileData("Testing is meh.")
                        }
            }
        );


        var loader = new RecentBenchesJsonFileLoader(
            fileSystemMock,
            jsonFileLoaderMock.Object);

        // Act
        var result = await loader.LoadAsync(testBenchFilePath);

        // Assert
        result.Should().HaveCount(1);
        result[0].BenchLabel.Should().Be(expectedBenchLabel);
        result[0].FilePath.Should().Be(testBenchFilePath);
        result[0].LastOpened.Should().Be(expectedLastOpened);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_File_Does_Not_Exist()
    {
        // Arrange
        var jsonFileLoaderMock = new Mock<IJsonFileLoader<RecentBenchesFileModel>>();
        var fileSystemMock = new MockFileSystem();

        var loader = new RecentBenchesJsonFileLoader(
            fileSystemMock,
            jsonFileLoaderMock.Object);

        // Act
        var result = await loader.LoadAsync("nonExistentPath");

        // Assert
        result.Should().BeEmpty();
    }
}
