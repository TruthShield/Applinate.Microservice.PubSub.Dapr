// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Applinate.PubSub;
    using Dapr.Client;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json.Serialization;

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
            //using var client = new DaprClientBuilder().Build(); // TODO: look into using dapr client here (and Grpc) instead of http

            var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("dapr-app-id", "order-processor");

            var endpointName   = PubSubProvider.GetEndpointName<TRequest>();
            var requestMessage = RequestMessage.Build<TRequest, TResponse>(request);
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestMessage), Encoding.UTF8, "application/json");
            var response       = await client.PostAsync($"{baseURL}/{endpointName}", requestContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotImplementedException(); // UNDONE: handle the serivce failure (throw?)
            }

            using var respStream = response.Content.ReadAsStream();
            using var reader     = new StreamReader(respStream);
            var data             = reader.ReadToEnd();
            var responseMessage  = JsonConvert.DeserializeObject<ResponseMessage>(data);
            var result           = JsonConvert.DeserializeObject<TResponse>(responseMessage.Payload) ?? throw new InvalidOperationException($"unable to deserialize {nameof(TResponse)}");

            return result;
        }
    }
}