using Microsoft.Extensions.DependencyInjection;
using Payeh.Mediator.Abstractions;
using Payeh.Mediator.Tests.Models;

namespace Payeh.Mediator.Tests;

public class MediatorTests : IClassFixture<MediatorTestSetup>
{
    private readonly IMediator _mediator;

    public MediatorTests(MediatorTestSetup setup)
    {
        _mediator = setup.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task TestHandlerInvocation()
    {
        // Arrange
        var request = new TestRequest { Input = "Hello, Mediator!" };

        // Act
        var result = await _mediator.Send(request);

        // Assert
        Assert.Equal("Processed: Hello, Mediator!", result);
    }
}