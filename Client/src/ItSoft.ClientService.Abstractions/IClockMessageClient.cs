using System;

namespace ItSoft.ClientService
{
    public interface IClockMessageClient<T>
    {
        event EventHandler<ClockDataEventArgs<byte[]>> DataReceived;
        event EventHandler ClientDisconnected;
        event EventHandler ClientConnected;
        void StartClient();
        void StopClient();
        void ChangeSettings(string ipAddress, int port);
    }
}