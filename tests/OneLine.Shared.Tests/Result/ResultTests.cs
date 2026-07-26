using FluentAssertions;
using OneLine.Shared.Domain.Result;

namespace OneLine.Shared.Tests.ResultTests;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldBeSuccessful()
    {
        var result = Result<string>.Success("Hello");
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be("Hello");
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldBeFailure()
    {
        var error = Error.NotFound("Test.NotFound", "Resource not found");
        var result = Result<string>.Failure(error);
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldBeSuccess()
    {
        Result<int> result = 42;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldBeFailure()
    {
        var error = Error.Validation("Test.Invalid", "Invalid data");
        Result<int> result = error;
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Test.Invalid");
    }

    [Fact]
    public void ErrorNone_ShouldHaveEmptyCodeAndMessage()
    {
        Error.None.Code.Should().BeEmpty();
        Error.None.Message.Should().BeEmpty();
        Error.None.Type.Should().Be(ErrorType.None);
    }

    [Fact]
    public void ErrorNotFound_ShouldHaveCorrectType()
    {
        var error = Error.NotFound("X.NotFound", "Not found");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void ErrorValidation_ShouldHaveCorrectType()
    {
        var error = Error.Validation("X.Invalid", "Invalid");
        error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void ErrorConflict_ShouldHaveCorrectType()
    {
        var error = Error.Conflict("X.Conflict", "Conflict");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Result_Void_Success_ShouldWork()
    {
        var result = Domain.Result.Result.Success();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Result_Void_Failure_ShouldWork()
    {
        var error = Error.Failure("X.Failed", "Failed");
        var result = Domain.Result.Result.Failure(error);
        result.IsFailure.Should().BeTrue();
    }
}
