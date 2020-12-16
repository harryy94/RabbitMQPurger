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
                HostName = "localhost"
            };

            var connection = rabbitMqClient.CreateConnection();

            IModel channel = connection.CreateModel();
            string jsonResponse;
            using (var webClient = new WebClient())
            {
                var request = $"http://{rabbitMqClient.HostName}:15672/api/queues";

                webClient.Credentials = new NetworkCredential(rabbitMqClient.UserName, rabbitMqClient.Password);
                jsonResponse = webClient.DownloadString(request);
            }

            var queues = JsonConvert.DeserializeObject<List<QueueResponse>>(jsonResponse);

            foreach (var queue in queues)
            {
                Console.WriteLine("Purging " + queue.Name);
                channel.QueuePurge(queue.Name);
            }
        }

        public class QueueResponse
        {
            public string Name { get; set; }
        }
    }
}
