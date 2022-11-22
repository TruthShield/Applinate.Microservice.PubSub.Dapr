//dapr run --app-port 7002 --app-id order-processor --app-protocol http --dapr-http-port 3501 -- dotnet run

Applinate.InitializationProvider.Initialize();

Console.WriteLine("hit 'enter' to shut down");
Console.ReadLine();

