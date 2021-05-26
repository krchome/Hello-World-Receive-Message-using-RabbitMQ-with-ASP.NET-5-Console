using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Receive
{
    class Receive
    {
        public static void Main()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetSection("AppConfig").GetSection("SmtpOptions").GetSection("Host").Value,
                UserName = configuration.GetSection("AppConfig").GetSection("SmtpOptions").GetSection("Username").Value,
                Password = configuration.GetSection("AppConfig").GetSection("SmtpOptions").GetSection("Password").Value,
                // VirtualHost = configuration.GetSection("AppConfig").GetSection("SmtpOptions").GetSection("RabbitMQHost").Value,
                Port = Convert.ToInt32(configuration.GetSection("AppConfig").GetSection("SmtpOptions").GetSection("Port").Value)
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}

