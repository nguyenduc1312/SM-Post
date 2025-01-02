using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Common;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Repositories;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }
        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (eventStream == null || !eventStream.Any())
                throw new AggregateNotFoundException("Invalid Aggreagte!");

            return eventStream.OrderBy(item => item.Version).Select(item => item.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();

            foreach(var @event in events)
            {
                expectedVersion++;
                @event.Version = expectedVersion;
                var eventModel = new EventModel()
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = expectedVersion,
                    EventType = @event.GetType().Name,
                    EventData = @event
                };

                await _eventStoreRepository.SaveAsync(eventModel);

                //var topic = "SocialMediaPostEvent";
                var topic = KafkaCommon.Topic;
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
