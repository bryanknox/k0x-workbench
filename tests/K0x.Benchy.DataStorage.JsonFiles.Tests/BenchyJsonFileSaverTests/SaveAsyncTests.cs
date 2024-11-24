using K0x.Benchy.DataStorage.Abstractions.Models;
using K0x.DataStorage.JsonFiles;
using Moq;

namespace K0x.Benchy.DataStorage.JsonFiles.Tests.BenchyJsonFileSaverTests;

public class SaveAsyncTests
{
    private readonly Mock<IJsonFileSaver<BenchyJsonFileModel>> _mockJsonFileSaver;
    private readonly BenchyJsonFileSaver _benchyJsonFileSaver;

    public SaveAsyncTests()
    {
        _mockJsonFileSaver = new Mock<IJsonFileSaver<BenchyJsonFileModel>>();

        _benchyJsonFileSaver = new BenchyJsonFileSaver(_mockJsonFileSaver.Object);
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
        await _benchyJsonFileSaver.SaveAsync(bench, filePath);

        // Assert
        _mockJsonFileSaver.Verify(
            saver => saver.SaveAsync(It.Is<BenchyJsonFileModel>(model => model.Bench == bench), filePath),
            Times.Once);
    }
}
