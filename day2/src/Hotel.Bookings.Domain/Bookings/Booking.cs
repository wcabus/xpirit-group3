using System;
using System.Threading.Tasks;
using Eventuous;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;
using static Hotel.Bookings.Domain.Services;

namespace Hotel.Bookings.Domain.Bookings {
    public class Booking : Aggregate<BookingState, BookingId> {
        public async Task BookRoom(
            BookingId       bookingId,
            string          guestId,
            RoomId          roomId,
            StayPeriod      period,
            Money           price,
            Money           prepaid,
            DateTimeOffset  bookedAt,
            IsRoomAvailable isRoomAvailable
        ) {
            EnsureDoesntExist();
            await EnsureRoomAvailable(roomId, period, isRoomAvailable);

            var outstanding = price - prepaid;

            Apply(
                new V1.RoomBooked(
                    bookingId,
                    guestId,
                    roomId,
                    period.CheckIn,
                    period.CheckOut,
                    price.Amount,
                    prepaid.Amount,
                    outstanding.Amount,
                    price.Currency,
                    outstanding.Amount == 0,
                    bookedAt
                )
            );
        }

        public void RecordPayment(
            Money          paid,
            string         paymentId,
            string         paidBy,
            DateTimeOffset paidAt
        ) {
            EnsureExists();

            var outstanding = State.Outstanding - paid;

            Apply(
                new V1.PaymentRecorded(
                    GetId(),
                    paid.Amount,
                    outstanding.Amount,
                    paid.Currency,
                    paymentId,
                    paidBy,
                    paidAt
                )
            );
        }

        static async Task EnsureRoomAvailable(RoomId roomId, StayPeriod period, IsRoomAvailable isRoomAvailable) {
            var roomAvailable = await isRoomAvailable(roomId, period);
            if (!roomAvailable) throw new DomainException("Room not available");
        }
    }
}