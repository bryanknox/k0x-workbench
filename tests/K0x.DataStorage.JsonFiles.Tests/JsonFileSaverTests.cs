using FluentAssertions;
using System.Text.Json;

namespace K0x.DataStorage.JsonFiles.Tests;

public class JsonFileSaverTests
{
    private class TestData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private readonly JsonFileSaver<TestData> _jsonFileService;
    private readonly string _tempTestFilesFolder;

    public JsonFileSaverTests()
    {
        _jsonFileService = new JsonFileSaver<TestData>();
        _tempTestFilesFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"TempTestFiles_{nameof(JsonFileSaverTests)}");

        // Delete and recreate the temp folder.
        //
        // Dev Note:
        // We do the cleanup before running each test method
        // to ensure that the test folder is empty before each test.
        // Any files created during the test are available for debugging
        // until the next test run.
        if (Directory.Exists(_tempTestFilesFolder))
        {
            Directory.Delete(_tempTestFilesFolder, true);
        }
        Directory.CreateDirectory(_tempTestFilesFolder);
    }

    [Fact]
    public async Task SaveAsync_ShouldWriteDataToFile()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "output.json");
        var testData = new TestData { Id = 1, Name = "Test" };

        // Act
        await _jsonFileService.SaveAsync(testData, filePath);

        // Assert
        var json = await File.ReadAllTextAsync(filePath);
        var result = JsonSerializer.Deserialize<TestData>(json);
        result.Should().BeEquivalentTo(testData);
    }
}
