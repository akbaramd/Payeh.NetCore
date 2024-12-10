using Payeh.Mediator.Abstractions.Handlers;
using Payeh.Mediator.Abstractions.Requests;

namespace Payeh.Mediator.Tests.Models;

public class TestRequest : IRequest<string>
{
    public string Input { get; set; }
}
public class TestRequestHandler : IRequestHandler<TestRequest, string>
{
    public async Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return await Task.FromResult($"Processed: {request.Input}");
    }
}