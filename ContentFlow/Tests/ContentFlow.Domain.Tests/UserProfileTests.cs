using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class UserProfileTests
{
    [Fact]
    public void Constructor_Should_ThrowArgumentException_WhenUserIdIsZeroOrNegative()
    {
        // Arrange & Act
        var act = () => new UserProfile(userId: -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId must be positive*");
    }

    [Fact]
    public void Constructor_Should_InitializePropertiesCorrectly()
    {
        // Arrange
        var userId = 42;
        var firstName = "Иван";
        var lastName = "Иванов";
        var bio = "Люблю писать код";
        var city = "Москва";
        var avatarUrl = "https://example.com/avatar.jpg";
        var birthDate = new DateOnly(1990, 5, 15);

        // Act
        var profile = new UserProfile(
            userId: userId,
            firstName: firstName,
            lastName: lastName,
            bio: bio,
            city: city,
            avatarUrl: avatarUrl,
            birthDate: birthDate);

        // Assert
        profile.UserId.Should().Be(userId);
        profile.FirstName.Should().Be(firstName);
        profile.LastName.Should().Be(lastName);
        profile.Bio.Should().Be(bio);
        profile.City.Should().Be(city);
        profile.AvatarUrl.Should().Be(avatarUrl);
        profile.BirthDate.Should().Be(birthDate);
        profile.Gender.Should().Be(Gender.Undefined);
        profile.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        profile.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        profile.DeletedAt.Should().BeNull();
        profile.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void UpdateProfile_Should_UpdateAllProvidedFieldsAndSetUpdatedAt()
    {
        // Arrange
        var profile = new UserProfile(userId: 1);
        var oldUpdatedAt = profile.UpdatedAt;

        // Act
        profile.UpdateProfile(
            firstName: "Анна",
            lastName: "Петрова",
            middleName: "Сергеевна",
            birthDate: new DateOnly(1985, 12, 1),
            city: "Санкт-Петербург",
            bio: "Backend-разработчик",
            gender: Gender.Female);

        // Assert
        profile.FirstName.Should().Be("Анна");
        profile.LastName.Should().Be("Петрова");
        profile.MiddleName.Should().Be("Сергеевна");
        profile.BirthDate.Should().Be(new DateOnly(1985, 12, 1));
        profile.City.Should().Be("Санкт-Петербург");
        profile.Bio.Should().Be("Backend-разработчик");
        profile.Gender.Should().Be(Gender.Female);
        profile.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
    }

    [Fact]
    public void MarkDeleted_Should_SetDeletedAtAndUpdatedAt_OnlyIfNotAlreadyDeleted()
    {
        // Arrange
        var profile = new UserProfile(userId: 1);
        var initialUpdatedAt = profile.UpdatedAt;

        // Act
        profile.MarkDeleted();

        // Assert
        profile.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        profile.UpdatedAt.Should().BeAfter(initialUpdatedAt.Value);
        profile.IsDeleted.Should().BeTrue();

        // Act again (should not change timestamps)
        var deletedAtBeforeSecondCall = profile.DeletedAt;
        var updatedAtBeforeSecondCall = profile.UpdatedAt;
        profile.MarkDeleted();

        // Assert
        profile.DeletedAt.Should().Be(deletedAtBeforeSecondCall);
        profile.UpdatedAt.Should().Be(updatedAtBeforeSecondCall);
    }

    [Fact]
    public void IsDeleted_Should_ReturnTrue_WhenDeletedAtIsSet()
    {
        // Arrange
        var profile = new UserProfile(userId: 1);
        profile.MarkDeleted();

        // Assert
        profile.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void IsDeleted_Should_ReturnFalse_WhenDeletedAtIsNull()
    {
        // Arrange
        var profile = new UserProfile(userId: 1);

        // Assert
        profile.IsDeleted.Should().BeFalse();
    }
}