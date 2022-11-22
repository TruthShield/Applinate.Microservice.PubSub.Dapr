namespace Acme
{
    using Applinate;

    [ServiceRequest(ServiceType.Orchestration)]
    public sealed class OrderCommand : ICommand
    {
        public OrderCommand(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}