using FluentAssertions;
using GameHub.API.Common.Results;

namespace GameHub.Tests.Unit.Common;

public class ResultTests
{
    [Fact]
    public void Should_CreateSuccessfulResult_When_SuccessIsCalled()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Should_CreateFailedResult_When_FailureIsCalled()
    {
        // Arrange
        var error = new Error(
            "Test.Error",
            "An error occurred.");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Success_Generic_ShouldCreateSuccessfulResultWithValue()
    {
        //Arrange
        const string value = "GameHub";

        //Act
        var result = Result<string>.Success(value);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Value.Should().Be(value);

    }

    [Fact]
    public void Failure_Generic_ShouldCreateFailedResultWithoutValue()
    {
        // Arrange
        var error = new Error(
            "Test.Error",
            "An error occurred.");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Value.Should().BeNull();
    }


}
