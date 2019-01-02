using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ItSoft.ClientService
{
    public class PodiumClockFrame
    {
        protected byte[] FrameStartBytes { get; set; } = {0x11};
        protected byte[] FrameEndBytes { get; set; } = {0x13};
        protected byte FrameTypeLength { get; set; } = 2;
        protected byte FrameTypePosition { get; set; } = 0;

        public byte[] Type { get; set; }
        public byte[] Body { get; set; }

        public static PodiumClockFrame Decode(byte[] frame)
        {
            var podiumClockFrame = new PodiumClockFrame();

            var startFrameExists = frame.Take(podiumClockFrame.FrameStartBytes.Length)
                .SequenceEqual(podiumClockFrame.FrameStartBytes);

            var frameEndExists = frame.Reverse().Take(podiumClockFrame.FrameEndBytes.Length).Reverse()
                .SequenceEqual(podiumClockFrame.FrameEndBytes);


            if (startFrameExists && frameEndExists)
            {
                var tmpFrame = frame.Except(podiumClockFrame.FrameStartBytes).Except(podiumClockFrame.FrameEndBytes);


                podiumClockFrame.Type = tmpFrame.Take(podiumClockFrame.FrameTypeLength).ToArray();
                podiumClockFrame.Body = frame.Skip(podiumClockFrame.FrameTypeLength).ToArray();


                return podiumClockFrame;
            }
            else
            {
                throw new ArgumentException("Frame does not have start and end markers.", nameof(frame));
            }
        }

        public override string ToString()
        {
            List<byte> frame = new List<byte>(FrameStartBytes);
            frame.AddRange(Type);
            frame.AddRange(Body);
            frame.AddRange(FrameEndBytes);

            return System.Text.Encoding.UTF8.GetString(frame.ToArray());
        }
    }

    public class ClockAndTimeFrame
    {
        public static readonly byte[] Type = {Convert.ToByte('T'), Convert.ToByte('1')};
        protected byte[] TextStartBytes { get; set; } = {0x2};
        protected byte[] TextEndBytes { get; set; } = {0x3};

        protected char Separator { get; set; } = ':';

        public bool IndicatorEnabled { get; set; }

        public char Sign { get; set; }

        public string Minutes { get; set; }

        public string Seconds { get; set; }

        public string Text { get; set; }

        public static ClockAndTimeFrame Decode(PodiumClockFrame baseFrame)
        {
            throw new NotImplementedException();
        }
    }


    public class FrameDecoder
    {
    }

    public class PodiumClockPodiumClockClient : IPodiumClockClient
    {
        public event EventHandler<ClockDataEventArgs> DataReceived;

        public PodiumClockPodiumClockClient(string ipAddress, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(ipAddress);
            var address = ipHostInfo.AddressList.First();
            IPEndPoint endPoint = new IPEndPoint(address, port);
        }
    }

    public class ClockDataEventArgs : EventArgs
    {
        public byte[] Bytes { get; set; }
    }
}