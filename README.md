# MQTT till Azure IoT Hub

## Beskrivning
Applikationen körs på en Raspberry Pi och fungerar som en gateway mellan MQTT och Azure IoT Hub. Den tar emot sensordata från MQTT-brokern och vidarebefordrar meddelandena till Azure IoT Hub som JSON.

En separat Azure-konsument kan sedan läsa och visa sensordatan från Azure Event Hubs.

## Lösningens delar
- **MqttService:** Tar emot sensordata från MQTT-brokern.
- **AzureIoTHubService:** Skickar sensordata vidare till Azure IoT Hub.
- **Programstart:** Startar tjänsterna och initierar kommunikationen.

## Teknik och beroenden
- **.NET 10:** Ramverk för applikationen.
- **MQTTnet:** Bibliotek för MQTT-kommunikation.
- **Microsoft.Azure.Devices.Client:** Bibliotek för kommunikation med Azure IoT Hub.
- **Microsoft.Extensions.Configuration.Json:** Används för att läsa inställningar från `appsettings.json`.

## Konfiguration
Skapa en lokal `appsettings.json` med följande inställningar:

- **MQTT:** Brokerns adress, port, eventuella inloggningsuppgifter och topic för sensordata.
- **Azure IoT Hub:** Enhetsanslutningssträngen (`DeviceConnectionString`) för en registrerad enhet.

Kontrollera att anslutningsuppgifterna är korrekta och att tjänsterna är tillgängliga. Filen `appsettings.json` ingår inte i versionshanteringen och måste konfigureras lokalt.

## Starta applikationen
1. Installera .NET 10 SDK på Raspberry Pi.
2. Kontrollera att MQTT-brokern och Azure IoT Hub är konfigurerade.
3. Kontrollera att `appsettings.json` innehåller rätt anslutningsuppgifter.
4. Öppna en terminal i projektets katalog.
5. Kör `dotnet run`.

## Dataflöde
ESP32 → MQTT-broker → Gateway på Raspberry Pi → Azure IoT Hub → Azure Event Hubs-konsument
