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

        // Delete and recreate the temp folder
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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task LoadAsync_ShouldThrowInvalidDataException_WhenFileContainsNoData(string emptyData)
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "empty.json");
        await File.WriteAllTextAsync(filePath, emptyData);

        // Act
        Func<Task> act = async () => await _jsonFileService.LoadAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>()
            .WithMessage($"File contains no data. FilePath: {filePath}");
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

    [Fact]
    public async Task LoadAsync_ShouldThrowJsonException_WhenFileContainsInvalidJson()
    {
        // Arrange
        var filePath = Path.Combine(_tempTestFilesFolder, "invalid.json");
        var invalidJson = "{ invalid json }";
        await File.WriteAllTextAsync(filePath, invalidJson);

        // Act
        Func<Task> act = async () => await _jsonFileService.LoadAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
}
