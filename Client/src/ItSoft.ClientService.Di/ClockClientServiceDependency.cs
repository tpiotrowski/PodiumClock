using System;
using Splat;

namespace ItSoft.ClientService.Di
{
    public class ClockClientServiceDependency
    {
        public static void Configure(IMutableDependencyResolver resolver,string ipAddress,int port)
        {
            var socketClockMessageClient = new SocketClockMessageClient(ipAddress, port);


            resolver.Register(() => socketClockMessageClient, typeof(IClockMessageClient<byte[]>));

            var socketDataDecoder = new SocketDataDecoder(socketClockMessageClient);

            resolver.Register(() => socketDataDecoder,typeof(ISocketDataDecoder));
            resolver.Register(()=>  new ClockAndTimeMessageDecoder(),typeof(IClockAndTimeMessageDecoder));





        }
    }
}
