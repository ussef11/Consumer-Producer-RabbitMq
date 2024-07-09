using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;


public class Program
{
    static string ApiUrl = "https://localhost:7115/";

    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "HahnCargoSim_NewOrders",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var newOrdersConsumer = new EventingBasicConsumer(channel);
        newOrdersConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonConvert.DeserializeObject<Order>(message);

            if (order.Id != 5)
            {
                Console.WriteLine($"Discarded order with id not equal to 5: Id={order.Id}");
                return;
            }

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}Order/Accept");
            request.Headers.Add("Authorization", $"Bearer {order.Token}");

            var content = new StringContent(JsonConvert.SerializeObject(new { orderId = 5 }), Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            Console.WriteLine($"Order Accepted: Id={order.Id}, OriginNodeId={order.OriginNodeId}, TargetNodeId={order.TargetNodeId}, Load={order.Load}, Value={order.Value}, DeliveryDateUtc={order.DeliveryDateUtc}, ExpirationDateUtc={order.ExpirationDateUtc}");
        };

        channel.BasicConsume(queue: "HahnCargoSim_NewOrders", autoAck: true, consumer: newOrdersConsumer);

        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }
}
