using System;

namespace ItSoft.ClientService
{
    public interface IClockMessageClient<T>
    {
        event EventHandler<ClockDataEventArgs<T>> DataReceived;
    }
}