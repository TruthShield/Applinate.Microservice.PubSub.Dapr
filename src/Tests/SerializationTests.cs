namespace Tests
{
    using Applinate.PubSub;
    using FluentAssertions;
    using Newtonsoft.Json;

    public class SerializationTests
    {
        [Fact]
        public void RequestMessageSerializationWorks()
        {
            var rm = new RequestMessage(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var ser = JsonConvert.SerializeObject(rm);
            var deser = JsonConvert.DeserializeObject<RequestMessage>(ser);

            deser.Should().Be(rm);

        }

        [Fact]
        public void ResponseMessageSerializationWorks()
        {
            var rm = new ResponseMessage(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var ser = JsonConvert.SerializeObject(rm);
            var deser = JsonConvert.DeserializeObject<ResponseMessage>(ser);

            deser.Should().Be(rm);

        }
    }
}