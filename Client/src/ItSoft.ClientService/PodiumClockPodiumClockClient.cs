using System;
using System.Linq;
using System.Net;

namespace ItSoft.ClientService
{
    public class PodiumClockPodiumClockClient : IPodiumClockClient
    {
        public event EventHandler<ClockDataEventArgs> DataReceived;

        public PodiumClockPodiumClockClient(string ipAddress, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(ipAddress);
            var address = ipHostInfo.AddressList.First();
            IPEndPoint endPoint = new IPEndPoint(address, port);
        }
    }
}