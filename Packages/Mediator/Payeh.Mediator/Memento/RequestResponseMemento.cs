using System;
using Payeh.Mediator.Abstractions.Memento;

namespace Payeh.Mediator.Memento
{
        public class RequestResponseMemento<TRequest, TResponse> : IRequestState<TRequest, TResponse>
        {
            public Guid RequestId { get; }
            public TRequest Request { get; }
            public TResponse Response { get;private set; }

            public RequestResponseMemento(Guid requestId, TRequest request)
            {
                RequestId = requestId;
                Request = request ?? throw new ArgumentNullException(nameof(request));
            }
            
            public void SetResponse(TResponse response)
            {
                Response = response;
            }
    }
}