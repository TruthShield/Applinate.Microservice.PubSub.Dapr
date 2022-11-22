namespace Tests
{
    using Acme;
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;

    public class ServiceTests:ApplinateTestBase
    {
        public ServiceTests() : base(ServiceType.Client)
        {
        }

        [Fact]
        public async Task QueryWorksWithoutDapr()
        {
            var input = 10;
            var result = await new OrderRequest(input).ExecuteAsync().ConfigureAwait(false);

            result.Value.Should().Be(input + 1);
        }

        [Fact]
        public async Task CommandWorksWithoutDapr()
        {
            var input = 10;
            var result = await new OrderCommand(input).ExecuteAsync().ConfigureAwait(false);

            result.Status.Should().Be(ResponseStatus.Success);
        }
    }
}