using FluentAssertions;
using Xunit;

namespace Payeh.Result.Tests
{
    public class PayehResultTests
    {
        [Fact]
        public void Result_Success_ShouldHaveIsSuccessTrue()
        {
            // Arrange & Act
            var result = PayehResult.Success();

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
            var result = PayehResult.Failure(errorMessage);

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
            var result = PayehResult<int>.Success(data);

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
            var result = PayehResult<int>.Failure(errorMessage);

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
            var result = PayehResult.Failure("An error occurred");

            // Act
            PayehResult<int> genericPayehResult = result;
            // Assert
            genericPayehResult.IsSuccess.Should().BeFalse();
            genericPayehResult.ErrorMessage.Should().Be(result.ErrorMessage);
            genericPayehResult.Data.Should().Be(default);
        }

        [Fact]
        public void ImplicitConversion_ResultTToResult_ShouldConvertSuccessfully()
        {
            // Arrange
            var genericResult = PayehResult<int>.Success(100);

            // Act
            PayehResult payehResult = genericResult;

            // Assert
            payehResult.IsSuccess.Should().BeTrue();
            payehResult.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Result_WithNullImplicitConversion_ShouldReturnNull()
        {
            // Arrange
            PayehResult nullPayehResult = null;

            // Act
            PayehResult<int> genericPayehResult = nullPayehResult;

            // Assert
            genericPayehResult.Should().BeNull();
        }

        [Fact]
        public void ResultT_WithNullImplicitConversion_ShouldReturnNull()
        {
            // Arrange
            PayehResult<int> nullGenericPayehResult = null;

            // Act
            PayehResult payehResult = nullGenericPayehResult;

            // Assert
            payehResult.Should().BeNull();
        }
    }
}
