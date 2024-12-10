using System;
using System.Collections.Generic;
using System.Linq;
using Payeh.Mediator.Abstractions.Memento;

namespace Payeh.Mediator.Memento
{
    public class MementoStore : IMementoStore
    {
        private readonly Dictionary<Guid, object> _mementos = new();

        public void Save<TRequest, TResponse>(IRequestState<TRequest, TResponse> memento)
        {
            if (memento == null)
                throw new ArgumentNullException(nameof(memento));

            if (_mementos.ContainsKey(memento.RequestId))
                throw new InvalidOperationException($"A memento with ID {memento.RequestId} already exists.");

            _mementos[memento.RequestId] = memento;
        }

        public void UpdateResponse<TRequest, TResponse>(Guid requestId, TResponse response)
        {
            if (!_mementos.TryGetValue(requestId, out var memento))
                throw new KeyNotFoundException($"No memento found with ID {requestId}.");

            if (memento is RequestResponseMemento<TRequest, TResponse> typedMemento)
            {
                typedMemento.SetResponse(response);
            }
            else
            {
                throw new InvalidCastException($"Memento with ID {requestId} cannot be cast to the requested types.");
            }
        }

        public IRequestState<TRequest, TResponse> Get<TRequest, TResponse>(Guid id)
        {
            if (!_mementos.TryGetValue(id, out var memento))
                throw new KeyNotFoundException($"No memento found with ID {id}.");

            if (memento is RequestResponseMemento<TRequest, TResponse> typedMemento)
            {
                return typedMemento;
            }

            throw new InvalidCastException($"Memento with ID {id} cannot be cast to the requested types.");
        }

        public IEnumerable<IRequestState<TRequest, TResponse>> GetAll<TRequest, TResponse>()
        {
            return _mementos.Values
                .OfType<RequestResponseMemento<TRequest, TResponse>>()
                .ToList();
        }
    }
}
