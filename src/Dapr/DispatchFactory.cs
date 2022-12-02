// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Applinate.PubSub;

    using Dapr.Client;

    using Newtonsoft.Json;

    using System;
    using System.Text.Encodings.Web;    
    using System.Text.Json.Serialization;
    using System.Text.Unicode;

    public record Order([property: JsonPropertyName("orderId")] int OrderId);

    [Intercept(-2100000000)]
    internal sealed class DispatchFactory : InterceptorFactoryBase
    {
        public override Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            ExecuteDelegate<TRequest, TResponse> next,
            TRequest request,
            CancellationToken cancellationToken) =>
            RequestContextProvider.ServiceType switch
            {
                ServiceType.Client =>
                    Publish<TRequest, TResponse>(request, cancellationToken),
                _ => base.ExecuteAsync(next, request, cancellationToken),
            };

        private static async Task<TResponse> DispatchCommand<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus
        {
            using var client = new DaprClientBuilder().Build();
            var message = PubSubProvider.BuildRequestMessage<TRequest, TResponse>(request);
            var topic = PubSubProvider.GetEndpointName<TRequest>();

            try
            {
                await client.PublishEventAsync("pubsub", topic, message, cancellationToken);
            }
            catch (Exception ex)
            {
                throw; // TODO: hanlde and return failure
            }

            dynamic response = CommandResponse.Success;

            return response;
        }

        private static async Task<TResponse> Publish<TRequest, TResponse>(
    TRequest request,
    CancellationToken cancellationToken)
    where TRequest : class, IReturn<TResponse>
    where TResponse : class, IHaveResponseStatus
        {
            using var client = new DaprClientBuilder().Build();

            string endpointName = PubSubProvider.GetEndpointName<TRequest>();
            RequestMessage requestMessage = RequestMessage.Build<TRequest, TResponse>(request);

            client.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All));
            
            var response = await client.InvokeMethodAsync<RequestMessage, ResponseMessage>("order-processor", endpointName, requestMessage, cancellationToken);

            var result = JsonConvert.DeserializeObject<TResponse>(response.Payload) ?? throw new InvalidOperationException($"unable to deserialize {nameof(TResponse)}");

            return result;
        }



    }
}