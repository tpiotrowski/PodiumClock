using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ItSoft.ClientService
{
    public class ClientService : IClientService
    {
        public ClientService(string ipAddress, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(ipAddress);
            var address = ipHostInfo.AddressList.First();
            IPEndPoint endPoint = new IPEndPoint(address, port);
        }

        
    }
}