using Eventuous;
using static Hotel.Payments.Domain.PaymentEvents;

namespace Hotel.Payments.Domain {
    public class Payment : Aggregate<PaymentState, PaymentId> {
        public void ProcessPayment(PaymentId paymentId, string bookingId, decimal amount, string method, string provider) {
            Apply(new PaymentRecorded(paymentId, bookingId, amount, method, provider));
        }
    }

    public record PaymentState : AggregateState<PaymentState, PaymentId> {
        public string  BookingId { get; init; }
        public decimal Amount    { get; init; }
    }

    public record PaymentId(string Value) : AggregateId(Value);
}