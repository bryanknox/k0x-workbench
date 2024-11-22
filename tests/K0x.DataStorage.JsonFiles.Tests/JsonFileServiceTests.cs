using FluentAssertions;
using System.Text.Json;

namespace K0x.DataStorage.JsonFiles.Tests;

public class JsonFileServiceTests
{
    private class TestData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    private readonly JsonFileService<TestData> _jsonFileService;
    private readonly string _tempTestFilesFolder;

    public JsonFileServiceTests()
    {
        _jsonFileService = new JsonFileService<TestData>();
        _tempTestFilesFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempTestFiles");

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
        var result = await _jsonFileService.LoadAsync(filePath);

        // Assert
        result.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public async Task LoadAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "nonexistent.json");

        // Act
        Func<Task> act = async () => await _jsonFileService.LoadAsync(filePath);

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
        Func<Task> act = async () => await _jsonFileService.LoadAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
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
