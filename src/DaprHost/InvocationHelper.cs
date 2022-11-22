namespace Applinate
{
    using Applinate.PubSub;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Applinate.Internals;

    internal static class InvocationHelper
    { 
        internal static async Task<ResponseMessage> InvokeAsync(HttpContext context)
        {
            var requestMessage = await GetRequestMessageAsync(context);
            var ctx = JsonConvert.DeserializeObject<RequestContext>(requestMessage.RequestContext);

            RequestContextHelper.SetCurrentRequestContext(ctx);

            var requestType  = Type.GetType(requestMessage.RequestTypeName);
            var responseType = Type.GetType(requestMessage.ResponseTypeName);

            if (requestType is null)
            {
                throw new InvalidOperationException($"expected type not found {requestMessage.RequestTypeName}.  Are you missing an assembly reference?");
            }

            if (responseType is null)
            {
                throw new InvalidOperationException($"expected type not found {requestMessage.ResponseTypeName}.  Are you missing an assembly reference?");
            }

            var resp = await ExecuteRequestAsync(requestType, responseType, requestMessage.Payload);

            return resp;
        }

        private static Task<ResponseMessage> ExecuteRequestAsync(Type? requestType, Type? responseType, string payload) =>
            typeof(InvocationHelper)
            .GetMethod(nameof(Go), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?
            .MakeGenericMethod(requestType, responseType)
            .Invoke(null, new object[] { payload }) as Task<ResponseMessage> ?? throw new InvalidOperationException("type not resolvable");

        private static async Task<ResponseMessage> Go<TRequest, TResponse>(string payload)
            where TRequest : class, IReturn<TResponse>
            where TResponse: class, IHaveResponseStatus
        {
            var req = JsonConvert.DeserializeObject<TRequest>(payload);
            var resp = await req.ExecuteAsync();

            var respMessage = new ResponseMessage(
                JsonConvert.SerializeObject(resp),
                JsonConvert.SerializeObject(RequestContextHelper.GetCurrentRequestContext()));

            return respMessage;
        }

        private static async Task<RequestMessage> GetRequestMessageAsync(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body);
            var value        = await reader.ReadToEndAsync();
            var request      = JsonConvert.DeserializeObject<RequestMessage>(value);

            if (string.IsNullOrWhiteSpace(request.RequestContext))
            {
                throw new InvalidOperationException("context required");
            }

            if (string.IsNullOrWhiteSpace(request.RequestTypeName))
            {
                throw new InvalidOperationException("request type required");
            }

            if (string.IsNullOrWhiteSpace(request.ResponseTypeName))
            {
                throw new InvalidOperationException("response type required");
            }

            if (string.IsNullOrWhiteSpace(request.Payload))
            {
                throw new InvalidOperationException("payload required");
            }

            return request;
        }
    }
}