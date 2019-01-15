using System;
using System.Text;

namespace ItSoft.ClientService.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClockMessageClient client = new SocketClockMessageClient("127.0.0.1", 8811);
           
            /*client.ClientConnected += (sender, eventArgs) => Console.WriteLine("Client connected");
            client.ClientDisconnected += (sender, eventArgs) => Console.WriteLine("Client disconnected");
            client.DataReceived += Client_DataReceived;*/


            SocketDataDecoder dataDecoder = new SocketDataDecoder(client);
            dataDecoder.FrameReceived += DataDecoder_FrameReceived;
            client.StartClient();



            while (true)
            {
            }
        }

        private static void DataDecoder_FrameReceived(object sender, ClockDataEventArgs<byte[]> e)
        {
            var clockAndTextMessage = ClockAndTimeMessageDecoder.Decode(BaseMessage.Decode(e.Message));





            UTF8Encoding utf8 = new UTF8Encoding();

            var s = utf8.GetString(e.Message);

            Console.WriteLine(clockAndTextMessage);
        }

        private static void Client_DataReceived(object sender, ClockDataEventArgs<byte[]> e)
        {
            UTF8Encoding utf8 = new UTF8Encoding();

            var s = utf8.GetString(e.Message);

            Console.WriteLine(s);
        }
    }
}