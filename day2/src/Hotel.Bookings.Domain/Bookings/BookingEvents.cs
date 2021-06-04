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
                float          BookingPrice,
                float          PrepaidAmount,
                float          OutstandingAmount,
                string         Currency,
                bool           Paid,
                DateTimeOffset BookingDate
            );

            public record PaymentRecorded(
                string BookingId, float PaidAmount, float Outstanding, string Currency, string PaymentId, string PaidBy,
                DateTimeOffset PaidAt
            );
        }

        public static void MapEvents() {
            TypeMap.AddType<V1.RoomBooked>("V1.RoomBooked");
            TypeMap.AddType<V1.PaymentRecorded>("V1.PaymentRecorded");
        }
    }
}