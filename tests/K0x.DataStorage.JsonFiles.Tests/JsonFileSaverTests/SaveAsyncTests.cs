using FluentAssertions;
using System.Text.Json;

namespace K0x.DataStorage.JsonFiles.Tests.JsonFileSaverTests;

public class SaveAsyncTests
{
    private class TestData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private readonly JsonFileSaver<TestData> _jsonFileService;
    private readonly string _tempTestFilesFolderPath;

    public SaveAsyncTests()
    {
        _jsonFileService = new JsonFileSaver<TestData>();
        _tempTestFilesFolderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"TempTestFiles_{nameof(SaveAsyncTests)}");

        // Delete and recreate the temp folder.
        //
        // Dev Note:
        // We do the cleanup before running each test method
        // to ensure that the test folder is empty before each test.
        // Any files created during the test are available for debugging
        // until the next test run.
        if (Directory.Exists(_tempTestFilesFolderPath))
        {
            Directory.Delete(_tempTestFilesFolderPath, true);
        }
        Directory.CreateDirectory(_tempTestFilesFolderPath);
    }

    [Fact]
    public async Task SaveAsync_ShouldWriteDataToFile()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolderPath, "output.json");
        var testData = new TestData { Id = 1, Name = "Test" };

        // Act
        await _jsonFileService.SaveAsync(testData, filePath);

        // Assert
        var json = await File.ReadAllTextAsync(filePath);
        var result = JsonSerializer.Deserialize<TestData>(json);
        result.Should().BeEquivalentTo(testData);
    }
}
