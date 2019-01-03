using System;

namespace ItSoft.ClientService
{
    public class ClockDataEventArgs : EventArgs
    {
        public byte[] Bytes { get; set; }
    }
}