using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eventuous.Projections.MongoDB;
using Eventuous.Projections.MongoDB.Tools;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Application.Bookings {
    public class MyBookingsProjection : MongoProjection<MyBookings> {
        public MyBookingsProjection(IMongoDatabase database, ILoggerFactory loggerFactory) :
            base(database, ProjectionSubscription.Id, loggerFactory) { }

        protected override ValueTask<Operation<MyBookings>> GetUpdate(object evt) {
            return evt switch {
                V1.RoomBooked e => UpdateOperationTask(
                    e.GuestId,
                    update => update.AddToSet(
                        x => x.Bookings,
                        new MyBookings.Booking(e.BookingId, e.CheckInDate, e.CheckOutDate, e.BookingPrice)
                    )
                ),
                V1.BookingFullyPaid e => UpdateOperationTask(
                    filter => filter.Eq(x => x.Id, e.GuestId) & 
                              filter.ElemMatch(x => x.Bookings, Builders<MyBookings.Booking>.Filter.Eq(x => x.BookingId, e.BookingId)),
                    update => update.Set(x => x.Bookings[-1].Paid, true)
                ),
                _ => NoOp
            };
        }
    }

    public record MyBookings : ProjectedDocument {
        public MyBookings(string id) : base(id) { }

        public List<Booking> Bookings { get; init; } = new();

        public record Booking(string BookingId, DateTimeOffset CheckInDate, DateTimeOffset CheckOutDate, decimal Price, bool Paid = false);
    }
}