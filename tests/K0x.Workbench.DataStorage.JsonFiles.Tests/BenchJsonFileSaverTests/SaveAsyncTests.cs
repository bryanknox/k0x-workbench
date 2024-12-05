using K0x.Workbench.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;
using Moq;

namespace K0x.Workbench.DataStorage.JsonFiles.Tests.BenchJsonFileSaverTests;

public class SaveAsyncTests
{
    private readonly Mock<IJsonFileSaver<BenchJsonFileModel>> _mockJsonFileSaver;
    private readonly BenchJsonFileSaver _benchJsonFileSaver;

    public SaveAsyncTests()
    {
        _mockJsonFileSaver = new Mock<IJsonFileSaver<BenchJsonFileModel>>();

        _benchJsonFileSaver = new BenchJsonFileSaver(_mockJsonFileSaver.Object);
    }

    [Fact]
    public async Task SaveAsync_ShouldWriteBenchToFile()
    {
        // Arrange
        var filePath = "C:/fake/file/path.json";
        var bench = new Bench
        {
            Label = "Test Bench",
            Trays = new List<Tray>
            {
                new Tray
                {
                    Label = "Tray 1",
                    Tools = new List<Tool>
                    {
                        new Tool { Label = "Tool 1", Command = "cmd1" },
                        new Tool { Label = "Tool 2", Command = "cmd2" }
                    }
                }
            }
        };

        // Act
        await _benchJsonFileSaver.SaveAsync(bench, filePath);

        // Assert
        _mockJsonFileSaver.Verify(
            saver => saver.SaveAsync(It.Is<BenchJsonFileModel>(model => model.Bench == bench), filePath),
            Times.Once);
    }
}
