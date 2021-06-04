using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using Eventuous;
using Eventuous.Producers.EventStoreDB;
using Eventuous.Shovel;
using Eventuous.Subscriptions;
using Eventuous.Subscriptions.EventStoreDB;
using Hotel.Payments.Domain;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Logging;

namespace Hotel.Payments.Integration {
    public class PaymentsShovel : ShovelService<PaymentsSubscription, EventStoreProducer> {
        public PaymentsShovel(
            EventStoreClient   eventStoreClient,
            ICheckpointStore   checkpointStore,
            IEventSerializer   eventSerializer,
            EventStoreProducer producer,
            ILoggerFactory     loggerFactory
        ) : base(
            PaymentsSubscription.Id,
            eventSerializer,
            (_, serializer, handlers, factory, _) => new PaymentsSubscription(
                eventStoreClient,
                checkpointStore,
                handlers,
                serializer,
                factory
            ),
            producer,
            message => new ValueTask<ShovelMessage>(Transform(message)),
            loggerFactory
        ) { }

        static ShovelMessage Transform(object original) {
            return original is PaymentEvents.PaymentRecorded evt
                ? new ShovelMessage(
                    "PaymentsIntegration",
                    new IntegrationEvents.BookingPaymentRecorded(evt.PaymentId, evt.BookingId, evt.Amount)
                )
                : null;
        }
    }

    public class PaymentsSubscription : AllStreamSubscription {
        public const string Id = "PaymentsAcl";

        public PaymentsSubscription(
            EventStoreClient           eventStoreClient,
            ICheckpointStore           checkpointStore,
            IEnumerable<IEventHandler> eventHandlers,
            IEventSerializer           eventSerializer,
            ILoggerFactory             loggerFactory
        ) : base(
            eventStoreClient,
            Id,
            checkpointStore,
            eventHandlers,
            eventSerializer,
            loggerFactory
        ) { }
    }

    public static class IntegrationEvents {
        public record BookingPaymentRecorded(string PaymentId, string BookingId, float Amount);

        public static void MapEvents() {
            TypeMap.AddType<BookingPaymentRecorded>("BookingPaymentRecorded");
        }
    }
}