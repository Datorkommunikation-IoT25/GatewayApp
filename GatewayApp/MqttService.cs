using Microsoft.Extensions.Configuration;
using MQTTnet;
using System.Text.Json;

public class MqttService
{
    private long _lastAzureSendTime = 0;


    public async Task StartAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string host = config["Mqtt:Host"] ?? "localhost";
        int port = int.Parse(config["Mqtt:Port"] ?? "1883");
        string username = config["Mqtt:Username"] ?? "";
        string password = config["Mqtt:Password"] ?? "";
        string topic = config["Mqtt:Topic"] ?? "sensor/temp";

        string connectionString =
            config["Azure:DeviceConnectionString"]
            ?? throw new InvalidOperationException(
                "Azure DeviceConnectionString saknas i appsettings.json.");

        var azureService = new AzureIoTHubService(connectionString);

        var factory = new MqttClientFactory();
        using var mqtt = factory.CreateMqttClient();

        mqtt.ApplicationMessageReceivedAsync += async e =>
        {
            string message = e.ApplicationMessage.ConvertPayloadToString();

            Console.WriteLine(
                $"{e.ApplicationMessage.Topic}: {message}");

            try
            {
                using var json = JsonDocument.Parse(message);
                var root = json.RootElement;

                string deviceId = root.GetProperty("deviceId").GetString()
                    ?? throw new InvalidOperationException("deviceId saknas.");

                double value = root.GetProperty("value").GetDouble();
                string unit = root.GetProperty("unit").GetString()
                    ?? throw new InvalidOperationException("unit saknas.");

                long now = Environment.TickCount64;

                if (now - _lastAzureSendTime < 10_000)
                {
                    return;
                }

                _lastAzureSendTime = now;

                await azureService.SendAsync(deviceId, value, unit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kunde inte behandla/skicka meddelandet: {ex.Message}");
            }
        };

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .WithCredentials(username, password)
            .Build();

        await mqtt.ConnectAsync(options);
        await mqtt.SubscribeAsync(topic);

        Console.WriteLine($"Ansluten till {host}. Lyssnar på {topic}.");

        await Task.Delay(Timeout.Infinite);
    }
}
