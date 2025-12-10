global using System;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;



namespace BackgroundPlayer
{
    class Program
    {

        static async Task Main(string[] args)
        {

            var connection = new AppServiceConnection();
            connection.AppServiceName = "PlayerControlService";
            connection.PackageFamilyName = "YourUwpPackageFamilyName";

            var status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                Console.WriteLine("Failed to connect");
                return;
            }

            // отправка сообщения UWP
            ValueSet request = new ValueSet();
            request["cmd"] = "hello";
            await connection.SendMessageAsync(request);

            // прием сообщений
            connection.RequestReceived += (s, e) =>
            {
                var cmd = e.Request.Message["cmd"].ToString();
                Console.WriteLine("Received from UWP: " + cmd);
            };

            Console.ReadLine();
        }
    }
}