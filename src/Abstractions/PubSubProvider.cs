namespace Applinate.PubSub
{
    using Applinate;
    using System.Threading;
    using System.Threading.Tasks;

    public static class PubSubProvider
    {
        public static RequestMessage BuildRequestMessage<TRequest, TResponse>(TRequest request)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus
        {
            // TODO: compress and encrypt
            return RequestMessage.Build<TRequest, TResponse>(request);
        }

        public static string GetEndpointName<TRequest>() where TRequest : class => GetTopicName(typeof(TRequest));

        public static string GetTopicName(Type t) => t?.FullName?.Replace('.', '-').ToKebabCase() ?? throw new ArgumentNullException(nameof(t)); // TODO: add version info

        public static Task HandleMessageRecievedAsync(ResponseMessage message, CancellationToken cancellationToken)
        {
            // deserialize and call
            throw new NotImplementedException();
        }
    }
}