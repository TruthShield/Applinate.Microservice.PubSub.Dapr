namespace Acme
{
    using Applinate;

    public sealed class OrderResponse : IHaveResponseStatus
    {
        public OrderResponse(int value, ResponseStatus responseStatus)
        {
            this.Value = value;
            this.Status = responseStatus;
        }

        public ResponseStatus Status { get; }
        public int Value { get; }
    }
}