using System;

namespace ItSoft.ClientService
{
    public interface ISocketDataDecoder : IService
    {
        event EventHandler<ClockDataEventArgs<byte[]>> FrameReceived;
    }
}