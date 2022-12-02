// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Applinate.PubSub;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using System.Data;

    internal sealed class Initializer : IInitialize
    {
        public bool SkipDuringTesting => throw new NotImplementedException();

        public void Initialize(bool testing = false)
        {
            var baseURL2 = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3501");

            var client2 = new HttpClient();

            client2.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //client2.DefaultRequestHeaders.Add("dapr-app-id", "order-processor");
            client2.DefaultRequestHeaders.Add("dapr-app-id", System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
            
                        var builder = WebApplication.CreateBuilder();

            builder.Services.AddDaprSidekick();// TODO: configuration?

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCloudEvents();

            GegisterOperations(app);

            Task.Run(() => app.RunAsync());
        }

        private static void GegisterOperations(WebApplication app)
        {
            var ops =
                (from c in TypeRegistry.Classes
                 where c.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReturn<>))
                 let isCommand = c.IsAssignableTo(typeof(ICommand))
                 select new { c, isCommand })
                    .GroupBy(x => x.isCommand, x => x.c)
                    .ToDictionary(x => x.Key, x => x.ToArray());

            if (ops.TryGetValue(true, out var commands))
            {
                app.MapSubscribeHandler();

                RegisterCommands(app, commands);
            }

            if (ops.TryGetValue(false, out var queries))
            {
                RegisterQueries(app, queries);
            }
        }

        private static void RegisterCommand(
            WebApplication app,
            Type commandType) =>
            RegisterPost(app, commandType)
            .WithTopic(
                "pubsub",
                PubSubProvider.GetTopicName(commandType));

        private static void RegisterCommands(WebApplication app, Type[] commands) =>
            commands.ForEach(c => RegisterCommand(app, c));

        private static RouteHandlerBuilder RegisterPost(
            WebApplication app,
            Type messageType) =>
            app.MapPost(
                $"/{PubSubProvider.GetTopicName(messageType)}",
                async (HttpContext context) =>
                {
                    var resp = await InvocationHelper.InvokeAsync(context);
                    return JsonConvert.SerializeObject(resp);
                });

        private static void RegisterQueries(WebApplication app, Type[] queries) =>
            queries.ForEach(q => RegisterPost(app, q));
    }
}