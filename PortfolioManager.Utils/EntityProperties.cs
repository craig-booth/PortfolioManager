using System;
using System.Collections.Generic;
using System.Text;


using PortfolioManager.EventStore;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.EventStore.Mongodb;

namespace PortfolioManager.Utils
{
    class EntityProperties
    {

        public void TestEventStreamProperties()
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var eventStore = new MongodbEventStore("mongodb://portfolio.boothfamily.id.au:27017");
            var eventStream = eventStore.GetEventStream("Test");

            var id = Guid.NewGuid();

            var properties = new Dictionary<string, string>();
            properties.Add("UserName", "craig@boothfamily.id.au");
            properties.Add("Type", "Admin");

            var events = new List<Event>();
            eventStream.Add(id, "Test", properties, events);

            properties.Remove("Type");
            eventStream.UpdateProperties(id, properties);


            var x = eventStream.Find("UserName", "craig@boothfamily.id.au");
        }

        public void TestEntityProperties()
        {

            var eventStore = new MongodbEventStore("mongodb://portfolio.boothfamily.id.au:27017");
            var eventStream = eventStore.GetEventStream<TestEntity>("Test");

            var factory = new DefaultEntityFactory<TestEntity>();

            var repository = new Repository<TestEntity>(eventStream, factory);

            var id = Guid.NewGuid();
            var myentity = new TestEntity(id);
            myentity.ChangeName("Craig");
            repository.Add(myentity);

            var myentity2 = repository.FindFirst("Name", "Craig");

            myentity2.ChangeName("Natalie");
            repository.Update(myentity2);


            var myentity3 = repository.FindFirst("Name", "Natalie");
        }
    }

    public class TestEntity : ITrackedEntity, ITrackedEntityWithProperties
    {

        public Guid Id { get; }

        private List<Event> _Events = new List<Event>();

        public string Name { get; private set; }

        public TestEntity(Guid id)
        {
            Id = id;
        }

        public void ChangeName(string name)
        {
            var @event = new ChangeNameEvent(Guid.NewGuid(), 0, name);
            Apply(@event);

            _Events.Add(@event);
        }

        public void Apply(ChangeNameEvent @event)
        {
            Name = @event.Name;
        }

        public void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }
        }

        public IEnumerable<Event> FetchEvents()
        {
            var events = new List<Event>(_Events);

            _Events.Clear();

            return events;
        }

        public IDictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();

            properties.Add("Name", Name);

            return properties;
        }
    }

    public class ChangeNameEvent : Event
    {
        public string Name { get; set; }

        public ChangeNameEvent(Guid id, int version, string name)
            : base(id, version)
        {
            Name = name;
        }
    }
}
