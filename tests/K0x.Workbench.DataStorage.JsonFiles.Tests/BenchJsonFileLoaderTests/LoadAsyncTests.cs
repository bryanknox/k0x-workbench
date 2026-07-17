using FluentAssertions;
using K0x.DataStorage.JsonFiles;
using K0x.Workbench.DataStorage.Abstractions.Models;
using Moq;

namespace K0x.Workbench.DataStorage.JsonFiles.Tests.BenchJsonFileLoaderTests;

public class LoadAsyncTests
{
    [Fact]
    public async Task LoadAsync_ShouldReturnBenchKit_WhenFileIsLoadedSuccessfully()
    {
        // Arrange
        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchJsonFileModel>>();
        var expectedBenchKit = new Kit
        {
            Label = "Test Bench",
            Kits = new List<Kit>
            {
                new Kit
                {
                    Label = "Test Kit",
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
        var benchJsonFileModel = new BenchJsonFileModel { Bench = expectedBenchKit };
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(benchJsonFileModel);

        var benchJsonFileLoader = new BenchJsonFileLoader(mockJsonFileLoader.Object);

        // Act
        var result = await benchJsonFileLoader.LoadAsync("testFilePath.json");

        // Assert
        result.Should().BeEquivalentTo(expectedBenchKit);
    }

    [Fact]
    public async Task LoadAsync_ShouldThrowException_WhenFileLoadingFails()
    {
        // Arrange
        const string expectedExceptionMessage = "Mock exception - File loading failed";

        var mockJsonFileLoader = new Mock<IJsonFileLoader<BenchJsonFileModel>>();
        mockJsonFileLoader.Setup(loader => loader.LoadAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var benchJsonFileLoader = new BenchJsonFileLoader(mockJsonFileLoader.Object);

        // Act
        Func<Task> act = async () => await benchJsonFileLoader.LoadAsync("testFilePath.json");

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(expectedExceptionMessage);
    }
}
