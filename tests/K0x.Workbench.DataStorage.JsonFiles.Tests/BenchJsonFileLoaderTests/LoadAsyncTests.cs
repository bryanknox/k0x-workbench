using FluentAssertions;
using K0x.Workbench.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;
using Moq;

namespace K0x.Workbench.DataStorage.JsonFiles.Tests.BenchJsonFileLoaderTests;

public class LoadAsyncTests
{
    [Fact]
    public async Task LoadAsync_ShouldReturnBench_WhenFileIsLoadedSuccessfully()
    {
        // Arrange
        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchJsonFileModel>>();
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
        var benchJsonFileModel = new BenchJsonFileModel { Bench = expectedBench };
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(benchJsonFileModel);

        var benchJsonFileLoader = new BenchJsonFileLoader(
            mockJsonFileLoader.Object,
            "testFilePath.json");

        // Act
        var result = await benchJsonFileLoader.LoadAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedBench);
    }

    [Fact]
    public async Task LoadAsync_ShouldThrowException_WhenFileLoadingFails()
    {
        // Arrange
        const string expectedExceptionMessage = "Mock exception - File loading failed";

        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchJsonFileModel>>();
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var benchJsonFileLoader = new BenchJsonFileLoader(
            mockJsonFileLoader.Object,
            "testFilePath.json");

        // Act
        Func<Task> act = async () => await benchJsonFileLoader.LoadAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(expectedExceptionMessage);
    }
}
