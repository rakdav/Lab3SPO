using Northwind.Queue.Models; // Для класса ProductQueueMessage
using RabbitMQ.Client; // Для класса ConnectionFactory
using RabbitMQ.Client.Events; // Для класса EventingBasicConsumer
using System.Text.Json; // Для класса JsonSerializer
string queueName = "product";
ConnectionFactory factory = new() { HostName = "localhost" };
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();
WriteLine("Declaring queue...");
QueueDeclareOk response = channel.QueueDeclare(
                                    queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
WriteLine("Queue name: {response.QueueName}, Message count: {response.MessageCount}, Consumer count: { response.ConsumerCount}.");
WriteLine("Waiting for messages...");
EventingBasicConsumer consumer = new(channel);
consumer.Received += (model, args) =>
{
    byte[] body = args.Body.ToArray();
    ProductQueueMessage? message = JsonSerializer
    .Deserialize<ProductQueueMessage>(body);
    if (message is not null)
    {
        WriteLine("Received product. Id: {message.Product.ProductId},Name: { message.Product.ProductName}, Message: { message.Text}");
    }
    else
    {
        WriteLine($"Received unknown: {args.Body.ToArray()}.");
    }
};
// Потребление начинается по мере поступления сообщений в очередь
channel.BasicConsume(queue: queueName,
autoAck: true,
consumer: consumer);
WriteLine(">>> Press Enter to stop consuming and quit. <<<");
ReadLine();
