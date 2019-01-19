using System;

namespace ItSoft.ClientService
{
    public partial interface IClockService : IService
    {
       
        event EventHandler<ClockAndTextMessageEventArgs> ClockAndTextMessageReceived;
    }

    public class ClockAndTextMessageEventArgs : EventArgs
    {
        public ClockAndTextMessage ClockAndTextMessage { get; set; }
    }
}