using ContentFlow.Domain.Entities;
using Xunit;

namespace ContentFlow.Domain.Tests;

public class TagTests
{
    [Fact]
    public void Constructor_Should_Set_Name_And_Generate_Slug()
    {
        // Arrange and Act
        var tag = new Tag("C# Programming");
        
        // Assert
        Assert.Equal("C# Programming", tag.Name);
        Assert.Equal("c-sharp-programming", tag.Slug);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_With_Empty_Name_Should_Throw_ArgumentException(string invalidName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Tag(invalidName));
        Assert.Contains("Name is required", exception.Message);
    }
    
    [Fact]
    public void Rename_Should_Update_Name_And_Slug()
    {
        // Arrange
        var tag = new Tag("Old Name");

        // Act
        tag.Rename("New Fancy Title");

        // Assert
        Assert.Equal("New Fancy Title", tag.Name);
        Assert.Equal("new-fancy-title", tag.Slug);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Rename_With_Empty_NewName_Should_Throw_ArgumentException(string invalidName)
    {
        // Arrange
        var tag = new Tag("Valid Name");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => tag.Rename(invalidName));
        Assert.Contains("Name is required", exception.Message);
    }
    
    [Fact]
    public void GenerateSlug_Should_Normalize_Multiple_Spaces()
    {
        // Act
        var slug = Tag.GenerateSlug("C#    is    awesome");

        // Assert
        Assert.Equal("c-sharp-is-awesome", slug);
    }

    [Fact]
    public void GenerateSlug_Should_Remove_Leading_Trailing_Dashes()
    {
        // Act
        var slug = Tag.GenerateSlug("  -- Test --  ");

        // Assert
        Assert.Equal("test", slug);
    }

    [Fact]
    public void GenerateSlug_Should_Handle_Special_Characters_Correctly()
    {
        // Act
        var slug = Tag.GenerateSlug("How to use .NET & C++?");

        // Assert
        Assert.Equal("how-to-use-dot-net-c-plus-plus", slug);
    }

    [Fact]
    public void GenerateSlug_Should_Return_Empty_For_Null_Input()
    {
        // Act
        var slug = Tag.GenerateSlug(null);

        // Assert
        Assert.Equal("", slug);
    }

    [Fact]
    public void GenerateSlug_Should_Return_Empty_For_Whitespace_Only()
    {
        // Act
        var slug = Tag.GenerateSlug("     ");

        // Assert
        Assert.Equal("", slug);
    }
}