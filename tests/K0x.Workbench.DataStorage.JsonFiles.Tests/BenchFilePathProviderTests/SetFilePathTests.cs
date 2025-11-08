using FluentAssertions;
using K0x.Workbench.DataStorage.JsonFiles;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace K0x.Workbench.DataStorage.JsonFiles.Tests.BenchFilePathProviderTests;

public class SetFilePathTests
{
    // Happy Path Tests

    [Fact]
    public void Should_Set_Valid_Absolute_Path()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var absolutePath = @"C:\Data\bench.json";

        // Act
        provider.SetFilePath(absolutePath);

        // Assert
        provider.FilePath.Should().Be(@"C:\Data\bench.json");
    }

    [Fact]
    public void Should_Set_Valid_Relative_Path_And_Convert_To_Absolute()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.Directory.SetCurrentDirectory(@"C:\CurrentDir");
        var provider = new BenchFilePathProvider(mockFileSystem);
        var relativePath = @"data\bench.json";

        // Act
        provider.SetFilePath(relativePath);

        // Assert
        provider.FilePath.Should().Be(@"C:\CurrentDir\data\bench.json");
    }

    [Fact]
    public void Should_Trim_Whitespace_From_Path()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var pathWithWhitespace = "  C:\\Data\\bench.json  ";

        // Act
        provider.SetFilePath(pathWithWhitespace);

        // Assert
        provider.FilePath.Should().Be(@"C:\Data\bench.json");
    }

    [Fact]
    public void Should_Remove_Surrounding_Double_Quotes()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var pathWithQuotes = "\"C:\\Data\\bench.json\"";

        // Act
        provider.SetFilePath(pathWithQuotes);

        // Assert
        provider.FilePath.Should().Be(@"C:\Data\bench.json");
    }

    [Fact]
    public void Should_Remove_Surrounding_Single_Quotes()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var pathWithQuotes = "'C:\\Data\\bench.json'";

        // Act
        provider.SetFilePath(pathWithQuotes);

        // Assert
        provider.FilePath.Should().Be(@"C:\Data\bench.json");
    }

    [Fact]
    public void Should_Handle_Path_With_Quotes_And_Whitespace()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var pathWithQuotesAndWhitespace = "  \"C:\\Data\\bench.json\"  ";

        // Act
        provider.SetFilePath(pathWithQuotesAndWhitespace);

        // Assert
        provider.FilePath.Should().Be(@"C:\Data\bench.json");
    }

    [Fact]
    public void Should_Handle_Path_With_Spaces_In_Name()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var pathWithSpaces = @"C:\My Data\bench file.json";

        // Act
        provider.SetFilePath(pathWithSpaces);

        // Assert
        provider.FilePath.Should().Be(@"C:\My Data\bench file.json");
    }

    // Null/Empty Input Tests

    [Fact]
    public void Should_Set_FilePath_To_Null_When_Input_Is_Null()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Act
        provider.SetFilePath(null);

        // Assert
        provider.FilePath.Should().BeNull();
    }

    [Fact]
    public void Should_Set_FilePath_To_Null_When_Input_Is_Empty_String()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Act
        provider.SetFilePath("");

        // Assert
        provider.FilePath.Should().BeNull();
    }

    [Fact]
    public void Should_Set_FilePath_To_Null_When_Input_Is_Whitespace()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Act
        provider.SetFilePath("   ");

        // Assert
        provider.FilePath.Should().BeNull();
    }

    [Fact]
    public void Should_Set_FilePath_To_Null_When_Input_Is_Only_Quotes()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Act
        provider.SetFilePath("\"\"");

        // Assert
        provider.FilePath.Should().BeNull();
    }

    [Fact]
    public void Should_Set_FilePath_To_Null_When_Input_Is_Quotes_With_Whitespace()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Act
        provider.SetFilePath("  \"  \"  ");

        // Assert
        provider.FilePath.Should().BeNull();
    }

    // Exception Tests

    [Fact]
    public void Should_Throw_ArgumentException_When_Path_Contains_Invalid_Characters()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        // Use a null character which is always invalid in paths
        var invalidPath = "C:\\Data\\bench\0.json";

        // Act
        var act = () => provider.SetFilePath(invalidPath);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*invalid characters*")
            .WithParameterName("filePath");
    }

    [Fact]
    public void Should_Throw_PathTooLongException_When_Path_Exceeds_Maximum_Length()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var tooLongPath = "C:\\" + new string('a', 32800);

        // Act
        var act = () => provider.SetFilePath(tooLongPath);

        // Assert
        act.Should().Throw<PathTooLongException>()
            .WithMessage("*too long*");
    }

    [Fact]
    public void Should_Throw_PathTooLongException_When_Resolved_Path_Exceeds_Maximum_Length()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        // Set a very long current directory
        var longDir = "C:\\" + new string('a', 32700);
        mockFileSystem.Directory.CreateDirectory(longDir);
        mockFileSystem.Directory.SetCurrentDirectory(longDir);

        var provider = new BenchFilePathProvider(mockFileSystem);
        var relativePath = new string('b', 100) + ".json";

        // Act
        var act = () => provider.SetFilePath(relativePath);

        // Assert
        act.Should().Throw<PathTooLongException>()
            .WithMessage("*too long*");
    }

    [Fact]
    public void Should_Throw_ArgumentException_For_Unsupported_Path_Format()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        // Use a tab character which is always invalid in paths
        var unsupportedPath = "C:\\Data\tfile\\bench.json";

        // Act
        var act = () => provider.SetFilePath(unsupportedPath);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*invalid characters*")
            .WithParameterName("filePath");
    }

    // Property Tests

    [Fact]
    public void FilePath_Should_Initially_Be_Null()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();

        // Act
        var provider = new BenchFilePathProvider(mockFileSystem);

        // Assert
        provider.FilePath.Should().BeNull();
    }

    [Fact]
    public void Should_Update_FilePath_When_SetFilePath_Called_Multiple_Times()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var firstPath = @"C:\Data\first.json";
        var secondPath = @"C:\Data\second.json";

        // Act
        provider.SetFilePath(firstPath);
        var firstResult = provider.FilePath;
        provider.SetFilePath(secondPath);
        var secondResult = provider.FilePath;

        // Assert
        firstResult.Should().Be(@"C:\Data\first.json");
        secondResult.Should().Be(@"C:\Data\second.json");
    }

    [Fact]
    public void Should_Allow_Resetting_FilePath_To_Null()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var provider = new BenchFilePathProvider(mockFileSystem);
        var validPath = @"C:\Data\bench.json";

        // Act
        provider.SetFilePath(validPath);
        var afterSet = provider.FilePath;
        provider.SetFilePath(null);
        var afterReset = provider.FilePath;

        // Assert
        afterSet.Should().Be(@"C:\Data\bench.json");
        afterReset.Should().BeNull();
    }
}
