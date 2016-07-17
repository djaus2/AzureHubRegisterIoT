using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Microsoft.Azure.Devices.Client;

static class AzureIoTHub
{
    //
    // This sample assumes the device has been connected to Azure with the IoT Dashboard
    //
    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

        //Not Using TPM:
    //private static deviceConnectionString = 
    //"HostName=<HostName.usr.azure-devices.net;DeviceId=MyDevice;SharedAccessKey=XXXXXX";


    public static async Task SendDeviceToCloudMessageAsync()
    {
        //Using TPM
        TpmDevice myDevice = new TpmDevice(0); // Use logical device 0 on the TPM
        string hubUri = myDevice.GetHostName();
        string deviceId = myDevice.GetDeviceId();
        string sasToken = myDevice.GetSASToken();

        var deviceClient = DeviceClient.Create(
            hubUri,
            AuthenticationMethodFactory.
                CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

        //Not using TPM
        //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);


#if WINDOWS_UWP
        var str = "Hello, Cloud from a secure UWP C# app!";
#else
        var str = "Hello, Cloud from a secure C# app!";
#endif
        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task SendDeviceToCloudMessageAsync(string msg)
    {
        TpmDevice myDevice = new TpmDevice(0); // Use logical device 0 on the TPM
        string hubUri = myDevice.GetHostName();
        string deviceId = myDevice.GetDeviceId();
        string sasToken = myDevice.GetSASToken();

        var deviceClient = DeviceClient.Create(
            hubUri,
            AuthenticationMethodFactory.
                CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

        //Not using TPM
        //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        var message = new Message(Encoding.ASCII.GetBytes(msg));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        TpmDevice myDevice = new TpmDevice(0); // Use logical device 0 on the TPM by default
        string hubUri = myDevice.GetHostName();
        string deviceId = myDevice.GetDeviceId();
        string sasToken = myDevice.GetSASToken();

        //Use TPM
        var deviceClient = DeviceClient.Create(
            hubUri,
            AuthenticationMethodFactory.
                CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

        //Not using TPM
        //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
