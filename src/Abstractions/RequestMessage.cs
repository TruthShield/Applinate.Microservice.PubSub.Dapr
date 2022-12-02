namespace Applinate.PubSub
{
    using Applinate;
    using Applinate.Internals;
    using Newtonsoft.Json;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [DataContract]
    [JsonObject]
    public sealed record RequestMessage
    {
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
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
        [JsonProperty(PropertyName = "p")]
        [JsonPropertyName("p")]
        public string Payload { get; init; } = string.Empty;

        [DataMember(Name ="requestType")]
        [JsonProperty(PropertyName = "requestType")]
        [JsonPropertyName("requestType")]
        public string RequestTypeName { get; init; } = string.Empty;

        [DataMember(Name ="responseType")]
        [JsonProperty(PropertyName = "responseType")]
        [JsonPropertyName("responseType")]
        public string ResponseTypeName { get; init; } = string.Empty;

        [DataMember(Name ="ctx")]
        [JsonProperty(PropertyName = "ctx")]
        [JsonPropertyName("ctx")]
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