using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.EventStore.Memory
{
    public class MemoryEventStore : IEventStore
    {
        private readonly Dictionary<string, IEventStream> _EventStreams;

        public MemoryEventStore()
        {
            _EventStreams = new Dictionary<string, IEventStream>();
        }

        public IEventStream GetEventStream(string collection)
        {
            if (_EventStreams.ContainsKey(collection))
                return _EventStreams[collection];
            else
            {
                var eventStream = new MemoryEventStream(collection);
                _EventStreams.Add(collection, eventStream);

                return eventStream;
            }
        }
 
    }
}
