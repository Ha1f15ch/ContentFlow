using ContentFlow.Domain.Entities;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldInitializeCategory()
    {
        var category = new Category("Technology", "All about tech");

        category.Name.Should().Be("Technology");
        category.Description.Should().Be("All about tech");
        category.Slug.Should().Be("technology");
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        var act = () => new Category(null!);
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        var act = () => new Category("");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Constructor_WithWhitespaceName_ShouldThrowArgumentException()
    {
        var act = () => new Category("   ");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Constructor_ShouldGenerateSlugFromName()
    {
        var category = new Category("  C# & .NET  ");

        category.Slug.Should().Be("c#-&-.net");
    }

    [Fact]
    public void Update_WithValidName_ShouldUpdateProperties()
    {
        var category = new Category("Old Name", "Old desc");
        var oldSlug = category.Slug;

        category.Update("New Name", "New desc");

        category.Name.Should().Be("New Name");
        category.Description.Should().Be("New desc");
        category.Slug.Should().Be("new-name");
        category.Slug.Should().NotBe(oldSlug);
    }

    [Fact]
    public void Update_WithNullName_ShouldThrowArgumentException()
    {
        var category = new Category("Valid");
        var act = () => category.Update(null!);
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Update_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        var category = new Category("Valid");
        var act = () => category.Update(invalidName);
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void GenerateSlug_ShouldHandleSpecialCharactersAndSpaces()
    {
        var slug = Category.GenerateSlug("   Machine Learning 101!   ");
        slug.Should().Be("machine-learning-101!");
    }

    [Fact]
    public void GenerateSlug_ShouldNotProduceLeadingOrTrailingDashes()
    {
        var slug = Category.GenerateSlug(" - C# Development - ");
        slug.Should().Be("c#-development");
    }
}