using System;

namespace ItSoft.ClientService
{
    public class ClockDataEventArgs<T> : EventArgs
    {
        public T Message { get; set; }
    }
}