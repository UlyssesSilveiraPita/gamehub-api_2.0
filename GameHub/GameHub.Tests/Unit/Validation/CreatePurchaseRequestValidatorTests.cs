using FluentAssertions;
using GameHub.API.Dtos.Purchases;
using GameHub.API.Validation.Purchases;

namespace GameHub.Tests.Unit.Validation;

public class CreatePurchaseRequestValidatorTests
{
    private readonly CreatePurchaseRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldReturnValid_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            GameProductId = 1,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.IsInvalid.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenGameProductIdIsZero()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            GameProductId = 0,
            Quantity = 1
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.Should().Contain(
            "GameProductId must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnInvalid_WhenQuantityIsZero()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            GameProductId = 1,
            Quantity = 0
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors.Should().Contain(
            "Quantity must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnAllErrors_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreatePurchaseRequest
        {
            GameProductId = 0,
            Quantity = 0
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().HaveCount(2);

        result.Errors.Should().Contain(
            "GameProductId must be greater than zero.");

        result.Errors.Should().Contain(
            "Quantity must be greater than zero.");
    }
}



