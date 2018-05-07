using IGrains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Streams;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Orleans test silo client
    /// </summary>
    public class Program
    {
        public static int count = 0;
        public static DateTime dt;
        static void Main(string[] args)
        {
            try
            {
                Console.ReadLine();
                RunMainAsync();

                Console.ReadLine();
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
            Console.ReadKey();
        }

        private static async void RunMainAsync()
        {
            try
            {
                dt = DateTime.Now;
                while (true)
                {
                    using (var client = await StartClientWithRetries())
                    {
                        while (true)
                        {

                            await DoClientWork(client);
                            //await Kafka(client);
                        }

                    }

                   

                }

                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 5)
        {
            int attempt = 0;
            IClusterClient client;
            while (true)
            {
                try
                {
                    var siloAddress = IPAddress.Loopback;
                    var gatewayPort = 30000;

                    client = new ClientBuilder()
                        .ConfigureDefaults()
                        .UseLocalhostClustering(gatewayPort)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IAuthorize).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddConsole())
                        .AddKafkaStreams("Kafka", opt =>
                        {
                            opt.TopicName = "Zop.Payment";
                            opt.KafkaConfig.Add("group.id", "Orleans.Streams.Kafka.Group");
                            opt.KafkaConfig.Add("socket.blocking.max.ms", 10);
                            opt.KafkaConfig.Add("socket.send.buffer.bytes", 100000);
                            //opt.KafkaConfig.Add("bootstrap.servers", "127.0.0.1:9092");
                            opt.KafkaConfig.Add("bootstrap.servers", "120.79.162.19:9092");

                        })
                        .Build();

                    await client.Connect();

                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(4));
                }
            }

            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            try
            {

                if (count == 1000)
                {

                    return;
                }

                var friend = client.GetGrain<IAuthorize>(0);
                RequestContext.Set("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVmZWZjNmE5YWM5MWFkZTYwYmFhMmRkZDM0YTkzNTJlIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTc5MDg5MjYsImV4cCI6MTUxNzkxMjUyNiwiaXNzIjoiaHR0cDovL2F1dGguem9wLmFsaW5nZmx5LmNvbSIsImF1ZCI6WyJodHRwOi8vYXV0aC56b3AuYWxpbmdmbHkuY29tL3Jlc291cmNlcyIsInVjX2FwaSIsImZtX2FwaSIsInBheV9hcGkiXSwiY2xpZW50X2lkIjoiWk9QVEVTVCIsInN4c2V4Ijoi5aWzIiwic3ViIjoiMyIsImF1dGhfdGltZSI6MTUxNzE5NjI3NSwiaWRwIjoibG9jYWwiLCJzY29wZSI6WyJwcm9maWxlIiwicGhvbmUiLCJvcGVuaWQiLCJ1Y19hcGkiLCJmbV9hcGkiLCJwYXlfYXBpIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.SAzFsWynN9J2DkqtItlgg33j0SA7nNDxcqeC6g1yhQ6eSE6Gzt5tEyI8cPFhv6YXdZ1U3VSLyLk_cPQt9eV32XSmxY4mDlq-dKTJIh0Rbt-XCythIuCoB-gYn40pV45X5Y8T-zd0aGs_w6ZNYbxyUaot5-_ZfCcvOQn6MwxMbOpgT1XwgMTeSpqtDqbgWEWArCkG5uBrdm5HekRe0qQNAXeQNk9ai0B_ZURNYvc2kzBQqut-OwcS6vtDeHNj8BflNHVa2P-Q7q_mG_W5PRtQ4NXraZwTaA-vsGAmR7bgJr6J4vgLQcCpDvJqG_SWqra6Bk2Wg2JXd5mKoXt7g7ildw");
                var response = await friend.SayHelloAsync("Good morning, my friend!");
                var auth = RequestContext.Get("AuthorizeResult");

                Console.WriteLine("\n\n{0}-{1}", response, DateTime.Now);
                Console.WriteLine("\n{0}", count);
                count++;
                Console.WriteLine("结束结束\n{0}", (DateTime.Now - dt).TotalMilliseconds);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static async Task Kafka(IClusterClient client)
        {
            count++;

            System.Threading.Thread.Sleep(1000);
            var dt = DateTime.Now;
            var StreamProvider = client.GetStreamProvider("Kafka");
            var Stream = StreamProvider.GetStream<string>(Guid.NewGuid(), "GrainImplicitStream");
            //await Stream.OnNextAsync("测试" + DateTime.Now, new AsyncStreamSequenceToken());
            await Stream.OnNextAsync("测试" + DateTime.Now);

            Console.WriteLine("\n\n{0}", DateTime.Now);
            Console.WriteLine("\n{0}--{1}", count,(DateTime.Now-dt).TotalMilliseconds);
        }
    }
}
