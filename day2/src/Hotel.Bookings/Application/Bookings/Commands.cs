using System;

namespace Hotel.Bookings.Application.Bookings {
    public static class BookingCommands {
        public record BookRoom(
            string         BookingId,
            string         GuestId,
            string         RoomId,
            DateTimeOffset CheckInDate,
            DateTimeOffset CheckOutDate,
            decimal        BookingPrice,
            decimal        PrepaidAmount,
            string         Currency,
            DateTimeOffset BookingDate
        );

        public record RecordPayment(
            string BookingId,
            decimal PaidAmount,
            string Currency,
            string PaymentId,
            string PaidBy
        );
    }
}