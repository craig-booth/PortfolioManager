using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.EventStore.Memory
{
    public class MemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, IEventStream> _EventStreams;

        public MemoryEventStore()
        {
            _EventStreams = new Dictionary<Guid, IEventStream>();
        }

        public IEventStream GetEventStream(Guid id)
        {
            if (_EventStreams.ContainsKey(id))
                return _EventStreams[id];
            else
            {
                var eventStream = new MemoryEventStream(id);
                _EventStreams.Add(id, eventStream);

                return eventStream;
            }
        }
 
    }
}
