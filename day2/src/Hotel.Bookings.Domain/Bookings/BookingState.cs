using Eventuous;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Domain.Bookings {
    public record BookingState : AggregateState<BookingState, BookingId> {
        public string     GuestId     { get; init; }
        public RoomId     RoomId      { get; init; }
        public StayPeriod Period      { get; init; }
        public Money      Price       { get; init; }
        public Money      Outstanding { get; init; }
        public bool       Paid        { get; init; }

        public override BookingState When(object evt) {
            return evt switch {
                V1.RoomBooked booked => this with {
                    Id = new BookingId(booked.BookingId),
                    RoomId = new RoomId(booked.RoomId),
                    Period = new StayPeriod(booked.CheckInDate, booked.CheckOutDate),
                    GuestId = booked.GuestId,
                    Price = new Money {Amount       = booked.BookingPrice, Currency      = booked.Currency},
                    Outstanding = new Money {Amount = booked.OutstandingAmount, Currency = booked.Currency}
                },
                V1.PaymentRecorded e => this with {
                    Outstanding = new Money { Amount = e.Outstanding, Currency = e.Currency }
                },
                V1.BookingFullyPaid => this with
                {
                    Paid = true
                },
                _ => this
            };
        }
    }

    public record PaymentRecord(string PaymentId, Money PaidAmount);

    public record DiscountRecord(Money Discount, string Reason);
}