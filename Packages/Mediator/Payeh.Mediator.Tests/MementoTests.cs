using Microsoft.Extensions.DependencyInjection;
using Payeh.Mediator.Abstractions;
using Payeh.Mediator.Abstractions.Memento;
using Payeh.Mediator.Tests.Models;
using Xunit;

public class MementoTests : IClassFixture<MediatorTestSetup>
{
    private readonly IMediator _mediator;
    private readonly IMementoStore _mementoStore;

    public MementoTests(MediatorTestSetup setup)
    {
        _mediator = setup.ServiceProvider.GetRequiredService<IMediator>();
        _mementoStore = setup.ServiceProvider.GetRequiredService<IMementoStore>();
    }

    [Fact]
    public async Task TestMementoCapture()
    {
        // Arrange
        var request = new TestRequest { Input = "Track me!" };

        // Act
        var result = await _mediator.Send(request);

        // Assert
        var mementos = _mementoStore.GetAll<TestRequest,string>();
        Assert.Single(mementos);
        Assert.Equal("Track me!", mementos.First().Request.Input);

        var responseMementos = _mementoStore.GetAll<TestRequest,string>();
        Assert.Single(responseMementos);
        Assert.Equal("Processed: Track me!", responseMementos.First().Response);
    }
}