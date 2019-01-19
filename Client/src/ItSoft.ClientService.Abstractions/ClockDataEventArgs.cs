using System;

namespace ItSoft.ClientService
{
    public class ClockDataEventArgs<T> : EventArgs
    {
        public T Message { get; set; }
    }

    public interface IClockAndTimeMessageDecoder
    {
        ClockAndTextMessage Decode(byte[] type, byte[] body);
    }
}