using IGrains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

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
                        }
                    }
                }
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

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("sub", "id"));
                var p= new ClaimsPrincipal(new ClaimsIdentity(claims));
                string json = JsonConvert.SerializeObject(claims);

                var friend = client.GetGrain<IAuthorize>(0);
                RequestContext.Set("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1MjgzNjE5MTUsImV4cCI6MTUyODM2NTUxNSwiaXNzIjoiaHR0cHM6Ly9hdXRoLnpnem9wLmNvbSIsImF1ZCI6WyJodHRwczovL2F1dGguemd6b3AuY29tL3Jlc291cmNlcyIsIklEQ19BUEkiLCJPVENfQVBJIiwiVUNfQVBJIl0sImNsaWVudF9pZCI6IjE3NTQwMDcyODg4ODc0MTg4OCIsInNjb3BlIjpbIklEQ19BUEkiLCJPVENfQVBJIiwiVUNfQVBJIl19.pES1QJjigtlIlMeOMXxwiwmXil6l10xlB6mAomSvLj9ZPePmCO-reQHNWJMcMxitZcQ0ikjXaglswH3ZEfpBRes14sVXbEVbGKDiyScbwfuyuVVILhHeq-ShtIl7xzr56uO-1J1r409EDJR4GHUT6P6ygdjUveXlSXXzWiN4XoKKWl0ZXZ3RGZ1G_JzdhUUAodjiApCTNNEIcQrv_lfPm5T5xavfAeOGSUPFcICu3Xb0YeknPTB76TgXJlTV6KvoDetvIGZrrRtMfbXPo8PAH7TUrySmvin5XoDJ-c_exPPR2C46EZ45hZ-_crJ_eFjRbu4g7zB4qYLEHqAjJojIjw");
                var response = await friend.SayHelloAsync("Good morning, my friend!");

                //var friend1 = client.GetGrain<IAuthorize>(1);
                //RequestContext.Set("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1MjgzNjE5MTUsImV4cCI6MTUyODM2NTUxNSwiaXNzIjoiaHR0cHM6Ly9hdXRoLnpnem9wLmNvbSIsImF1ZCI6WyJodHRwczovL2F1dGguemd6b3AuY29tL3Jlc291cmNlcyIsIklEQ19BUEkiLCJPVENfQVBJIiwiVUNfQVBJIl0sImNsaWVudF9pZCI6IjE3NTQwMDcyODg4ODc0MTg4OCIsInNjb3BlIjpbIklEQ19BUEkiLCJPVENfQVBJIiwiVUNfQVBJIl19.pES1QJjigtlIlMeOMXxwiwmXil6l10xlB6mAomSvLj9ZPePmCO-reQHNWJMcMxitZcQ0ikjXaglswH3ZEfpBRes14sVXbEVbGKDiyScbwfuyuVVILhHeq-ShtIl7xzr56uO-1J1r409EDJR4GHUT6P6ygdjUveXlSXXzWiN4XoKKWl0ZXZ3RGZ1G_JzdhUUAodjiApCTNNEIcQrv_lfPm5T5xavfAeOGSUPFcICu3Xb0YeknPTB76TgXJlTV6KvoDetvIGZrrRtMfbXPo8PAH7TUrySmvin5XoDJ-c_exPPR2C46EZ45hZ-_crJ_eFjRbu4g7zB4qYLEHqAjJojIjw");
                //var response1 = await friend1.SayHelloAsync("Good morning, my friend1   friend1!");

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

      
    }
}
