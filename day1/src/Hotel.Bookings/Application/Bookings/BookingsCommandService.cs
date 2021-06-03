using System;
using System.Threading.Tasks;
using CoreLib;
using Hotel.Bookings.Domain;
using Hotel.Bookings.Domain.Bookings;

namespace Hotel.Bookings.Application.Bookings {
    public class BookingsCommandService : CommandService<Booking, BookingId, BookingState> {
        public BookingsCommandService(IAggregateStore store, Services.IsRoomAvailable isRoomAvailable) : base(store) {
            OnNew<BookingCommands.BookRoom>(
                async (booking, cmd) => {
                    await booking.BookRoom(
                        new BookingId(cmd.BookingId),
                        cmd.GuestId,
                        new RoomId(cmd.RoomId),
                        new StayPeriod(cmd.CheckInDate, cmd.CheckOutDate),
                        new Money(cmd.BookingPrice, cmd.Currency),
                        isRoomAvailable
                    );
                });
        }
    }
}
