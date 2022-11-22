namespace Acme
{
    using Applinate;

    [ServiceRequest(ServiceType.Orchestration)]
    public sealed class OrderRequest:IReturn<OrderResponse>
    {
        public OrderRequest(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}