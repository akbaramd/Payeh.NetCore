using FluentAssertions;
using Xunit;

namespace Payeh.Result.Tests
{
    public class ResultTests
    {
        [Fact]
        public void Result_Success_ShouldHaveIsSuccessTrue()
        {
            // Arrange & Act
            var result = Result.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Result_Failure_ShouldHaveIsFailureTrue()
        {
            // Arrange
            var errorMessage = "An error occurred";

            // Act
            var result = Result.Failure(errorMessage);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void ResultT_Success_ShouldContainData()
        {
            // Arrange
            var data = 42;

            // Act
            var result = Result<int>.Success(data);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Data.Should().Be(data);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void ResultT_Failure_ShouldHaveErrorMessage()
        {
            // Arrange
            var errorMessage = "Invalid data";

            // Act
            var result = Result<int>.Failure(errorMessage);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be(errorMessage);
            result.Data.Should().Be(default);
        }

        [Fact]
        public void ImplicitConversion_ResultToResultT_ShouldConvertSuccessfully()
        {
            // Arrange
            var result = Result.Failure("An error occurred");

            // Act
            Result<int> genericResult = result;
            // Assert
            genericResult.IsSuccess.Should().BeFalse();
            genericResult.ErrorMessage.Should().Be(result.ErrorMessage);
            genericResult.Data.Should().Be(default);
        }

        [Fact]
        public void ImplicitConversion_ResultTToResult_ShouldConvertSuccessfully()
        {
            // Arrange
            var genericResult = Result<int>.Success(100);

            // Act
            Result result = genericResult;

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Result_WithNullImplicitConversion_ShouldReturnNull()
        {
            // Arrange
            Result nullResult = null;

            // Act
            Result<int> genericResult = nullResult;

            // Assert
            genericResult.Should().BeNull();
        }

        [Fact]
        public void ResultT_WithNullImplicitConversion_ShouldReturnNull()
        {
            // Arrange
            Result<int> nullGenericResult = null;

            // Act
            Result result = nullGenericResult;

            // Assert
            result.Should().BeNull();
        }
    }
}
