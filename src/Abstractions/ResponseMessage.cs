namespace Applinate.PubSub
{
    using Applinate;
    using Applinate.Internals;
    using Newtonsoft.Json;
    using System.Text.Json.Serialization;

    public sealed record ResponseMessage // TODO: add fault info for re-throw on client.
    {
        [System.Text.Json.Serialization.JsonConstructor]
        public ResponseMessage(string payload, string requestContext)
        {
            Payload = payload;
            RequestContext = requestContext;
        }

        public ResponseMessage() { }

        [property: JsonPropertyName("p")]
        public string Payload { get; init; } = string.Empty;

        [property: JsonPropertyName("ctx")]
        public string RequestContext { get; init; } = string.Empty;

        public static ResponseMessage Build<TRequest, TResponse>(TRequest request)
            where TRequest: class, IReturn<TResponse>
            where TResponse: class, IHaveResponseStatus
        {
            // TODO: encryption of payload
            return new(
                JsonConvert.SerializeObject(request),
                JsonConvert.SerializeObject(RequestContextHelper.GetCurrentRequestContext()));
        }
    }
}