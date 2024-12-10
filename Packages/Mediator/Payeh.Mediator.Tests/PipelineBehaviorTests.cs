using Microsoft.Extensions.DependencyInjection;
using Payeh.Mediator.Abstractions;
using Payeh.Mediator.Abstractions.Pipeline;
using Payeh.Mediator.Tests.Models;

namespace Payeh.Mediator.Tests;

using Moq;
using Xunit;

public class PipelineBehaviorTests : IClassFixture<MediatorTestSetup>
{
    private readonly IMediator _mediator;

    public PipelineBehaviorTests(MediatorTestSetup setup)
    {
        _mediator = setup.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task TestPipelineBehaviorInvocation()
    {
        // Arrange
        var mockBehavior = new Mock<IRequestPipelineBehavior<TestRequest, string>>();
        var request = new TestRequest { Input = "Pipeline Test" };

        mockBehavior.Setup(b => b.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>(), It.IsAny<Func<TestRequest, CancellationToken, Task<string>>>()))
            .ReturnsAsync("Intercepted");

        // Act
        var result = await _mediator.Send(request);

        // Assert
        Assert.Equal("Processed: Pipeline Test", result); // Ensure original behavior is not overridden
    }
}
