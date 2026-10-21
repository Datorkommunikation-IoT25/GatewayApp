using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Text.Json;

public class AzureIoTHubService
{
    private readonly DeviceClient _client;

    public AzureIoTHubService(string connectionString)
    {
        _client = DeviceClient.CreateFromConnectionString(connectionString);
    }

    public async Task SendAsync(string deviceId, double value, string unit)
    {
        var telemetry = new
        {
            deviceId,
            value,
            unit
        };

        string json = JsonSerializer.Serialize(telemetry);

        using var message = new Message(
            Encoding.UTF8.GetBytes(json));

        message.ContentType = "application/json";
        message.ContentEncoding = "utf-8";

        await _client.SendEventAsync(message);

        Console.WriteLine($"Skickat till Azure: {json}");
    }
}