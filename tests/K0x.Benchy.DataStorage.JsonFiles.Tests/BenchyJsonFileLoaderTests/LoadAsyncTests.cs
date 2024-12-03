using FluentAssertions;
using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;
using Moq;

namespace K0x.Benchy.DataStorage.JsonFiles.Tests.BenchyJsonFileLoaderTests;

public class LoadAsyncTests
{
    [Fact]
    public async Task LoadAsync_ShouldReturnBench_WhenFileIsLoadedSuccessfully()
    {
        // Arrange
        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchyJsonFileModel>>();
        var expectedBench = new Bench
        {
            Label = "Test Bench",
            Trays = new List<Tray>
            {
                new Tray
                {
                    Label = "Test Tray",
                    Tools = new List<Tool>
                    {
                        new Tool
                        {
                            Label = "Test Tool",
                            Command = "Test Command",
                            WorkingDirectory = "Test Directory"
                        }
                    }
                }
            }
        };
        var benchyJsonFileModel = new BenchyJsonFileModel { Bench = expectedBench };
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(benchyJsonFileModel);

        var benchyJsonFileLoader = new BenchyJsonFileLoader(
            mockJsonFileLoader.Object,
            "testFilePath.json");

        // Act
        var result = await benchyJsonFileLoader.LoadAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedBench);
    }

    [Fact]
    public async Task LoadAsync_ShouldThrowException_WhenFileLoadingFails()
    {
        // Arrange
        const string expectedExceptionMessage = "Mock exception - File loading failed";

        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchyJsonFileModel>>();
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var benchyJsonFileLoader = new BenchyJsonFileLoader(
            mockJsonFileLoader.Object,
            "testFilePath.json");

        // Act
        Func<Task> act = async () => await benchyJsonFileLoader.LoadAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(expectedExceptionMessage);
    }
}
