//dapr run  --app-id checkout --app-protocol http --dapr-http-port 3500 -- dotnet run
//
Applinate.InitializationProvider.Initialize();

Console.WriteLine($@"
---------------------------------
click enter to proceed
---------------------------------");

Console.ReadLine();

Console.WriteLine($@"
Query (RPC request-response)
---------------------------------");

var c = new Applinate.ServiceClient(
    Guid.NewGuid(),
    new(SequentialGuid.NewGuid(), 0, 0, 1));

c.StartNewConversation();

var result1 = await new Acme.OrderRequest(1).ExecuteAsync();

Console.WriteLine(result1.Value);

Console.WriteLine($@"
---------------------------------
Command (PubSub dispatch)
---------------------------------");

var result2 = await new Acme.OrderCommand(65).ExecuteAsync();

_ = result2;

Console.WriteLine(
$@"---------------------------------
click enter to close
---------------------------------");

Console.Read();