namespace Applinate.PubSub
{
    using Applinate;
    using Applinate.Internals;
    using Newtonsoft.Json;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed record RequestMessage
    {
        public RequestMessage(
            string payload, 
            string requestTypeName,
            string responseTypeName,
            string requestContext)
        {
            Payload = payload ?? string.Empty;
            RequestTypeName = requestTypeName ?? string.Empty;
            ResponseTypeName = responseTypeName ?? string.Empty;
            RequestContext = requestContext ?? string.Empty;
        }

        public RequestMessage() { }

        [DataMember(Name = "p")]
        public string Payload { get; init; } = string.Empty;

        [DataMember(Name ="requestType")]
        public string RequestTypeName { get; init; } = string.Empty;

        [DataMember(Name ="responseType")]
        public string ResponseTypeName { get; init; } = string.Empty;

        [DataMember(Name ="ctx")]
        public string RequestContext { get; init; } = string.Empty;

        public static RequestMessage Build<TRequest, TResponse>(TRequest request)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus
        {
            // TODO: encryption of payload (if configured)
            return new(
                JsonConvert.SerializeObject(request),
                typeof(TRequest).AssemblyQualifiedName,
                typeof(TResponse).AssemblyQualifiedName,
                JsonConvert.SerializeObject(RequestContextHelper.GetCurrentRequestContext()));
        }
    }
}