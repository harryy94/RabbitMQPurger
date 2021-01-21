using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMQPurger
{
    class Program
    {
        static void Main(string[] args)
        {
            var rabbitMqClient = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost",
            };

            // Local
            //var scheme = "http";
            //var port = 15672;

            // ENvironment
            var scheme = "https";
            var port = 443;

            string jsonResponse;
            using (var webClient = new WebClient())
            {
                var request = $"{scheme}://{rabbitMqClient.HostName}:{port}/api/queues";

                webClient.Credentials = new NetworkCredential(rabbitMqClient.UserName, rabbitMqClient.Password);

                jsonResponse = webClient.DownloadString(request);

                var queues = JsonConvert.DeserializeObject<List<QueueResponse>>(jsonResponse);

                foreach (var queue in queues)
                {
                    request = $"{scheme}://{rabbitMqClient.HostName}:{port}/api/queues/%2F/{queue.Name}/contents";
                    webClient.UploadString(request, "DELETE", "");
                    Console.WriteLine("Purging " + queue.Name);
                }
            }
        }

        public class QueueResponse
        {
            public string Name { get; set; }
        }
    }
}
