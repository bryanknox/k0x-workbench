using FluentAssertions;
using System.Text.Json;

namespace K0x.DataStorage.JsonFiles.Tests;

public class JsonFileLoaderTests
{
    private class TestData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private readonly JsonFileLoader<TestData> _jsonFileLoader;

    private readonly string _tempTestFilesFolder;

    public JsonFileLoaderTests()
    {
        _jsonFileLoader = new JsonFileLoader<TestData>();
        _tempTestFilesFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"TempTestFiles_{nameof(JsonFileLoaderTests)}");

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
    public async Task LoadAsync_ShouldReturnData_WhenFileContainsValidJson()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "test.json");
        var testData = new TestData { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(testData);
        await File.WriteAllTextAsync(filePath, json);

        // Act
        var result = await _jsonFileLoader.LoadAsync(filePath);

        // Assert
        result.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task LoadAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "nonexistent.json");

        // Act
        Func<Task> act = async () => await _jsonFileLoader.LoadAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Theory]
    [InlineData("{ invalid json }")]
    [InlineData("just text")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task LoadAsync_ShouldThrowJsonException_WhenFileContainsInvalidJson(string invalidJson)
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "invalid.json");
        await File.WriteAllTextAsync(filePath, invalidJson);

        // Act
        Func<Task> act = async () => await _jsonFileLoader.LoadAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
}
