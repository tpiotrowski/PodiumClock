using System;
using Splat;

namespace ItSoft.ClientService.Di
{
    public class Class1
    {
        public void ConfigureClockClient(IMutableDependencyResolver resolver,string ipAddress,int port)
        {
            var socketClockMessageClient = new SocketClockMessageClient(ipAddress, port);
            resolver.Register(() => socketClockMessageClient, typeof(IClockMessageClient<byte[]>));
            resolver.Register(() => new SocketDataDecoder(socketClockMessageClient),typeof(ISocketDataDecoder));
            resolver.Register(()=>  new ClockAndTimeMessageDecoder(),typeof(IClock)));





        }
    }
}
