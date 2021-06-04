using System;
using Eventuous;

namespace Hotel.Bookings.Domain.Bookings {
    public static class BookingEvents {
        public static class V1 {
            public record RoomBooked(
                string         BookingId,
                string         GuestId,
                string         RoomId,
                DateTimeOffset CheckInDate,
                DateTimeOffset CheckOutDate,
                decimal        BookingPrice,
                decimal        PrepaidAmount,
                decimal        OutstandingAmount,
                string         Currency,
                DateTimeOffset BookingDate
            );

            public record PaymentRecorded(
                string BookingId, decimal PaidAmount, decimal Outstanding, string Currency, string PaymentId, string PaidBy,
                DateTimeOffset PaidAt
            );

            public record BookingFullyPaid(DateTimeOffset When);
        }

        public static void MapEvents() {
            TypeMap.AddType<V1.RoomBooked>("V1.RoomBooked");
            TypeMap.AddType<V1.PaymentRecorded>("V1.PaymentRecorded");
            TypeMap.AddType<V1.BookingFullyPaid>("V1.BookingFullyPaid");
        }
    }
}