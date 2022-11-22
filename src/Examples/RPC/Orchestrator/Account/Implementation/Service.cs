namespace Implementation
{
    using Acme;
    using Applinate;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Service : IRequestHandler<OrderRequest, OrderResponse>, ICommandHandler<OrderCommand>
    {
        public Task<OrderResponse> ExecuteAsync(OrderRequest request, CancellationToken cancellationToken = default)
        {
            WriteLine("OrderRequest received : " + request.Value);

            var result = new OrderResponse(request.Value + 1, ResponseStatus.Success);

            return Task.FromResult(result);
        }

        public Task<CommandResponse> ExecuteAsync(OrderCommand request, CancellationToken cancellationToken = default)
        {
            WriteLine("OrderCommand recieved : " + request.Value);

            return Task.FromResult(CommandResponse.Success);
        }

        private static void WriteLine(string value)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}