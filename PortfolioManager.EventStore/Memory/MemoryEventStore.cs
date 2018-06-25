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

        public IEventStream GetEventStream(string streamName)
        {
            if (_EventStreams.ContainsKey(streamName))
                return _EventStreams[streamName];
            else
            {
                var eventStream = new MemoryEventStream(streamName);
                _EventStreams.Add(streamName, eventStream);

                return eventStream;
            }
        }
 
    }
}
