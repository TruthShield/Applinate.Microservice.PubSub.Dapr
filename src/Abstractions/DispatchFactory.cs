namespace Applinate.PubSub
{
    using Applinate;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class DispatchFactory:InterceptorFactoryBase
    {
        public override Task<TResponse> ExecuteAsync<TRequest, TResponse>(ExecuteDelegate<TRequest, TResponse> next, TRequest request, CancellationToken cancellationToken)
        {
            switch(RequestContextProvider.ServiceType)
            {
                case ServiceType.Client:
                    throw new NotImplementedException("pack and dispatch?");
                default:
                    return base.ExecuteAsync(next, request, cancellationToken);
            }
        }
    }
}