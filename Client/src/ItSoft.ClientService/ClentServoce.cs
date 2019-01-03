using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
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
        protected byte TextStartBytes { get; set; } = 0x2;
        protected byte TextEndBytes { get; set; } = 0x3;

        protected char Separator { get; set; } = ':';

        public bool IndicatorEnabled { get; set; }

        public char Sign { get; set; }

        public string Minutes { get; set; }

        public string Seconds { get; set; }

        public string Text { get; set; }

        public string Time => $"{Sign} {Minutes}{Separator}{Seconds}";

        public static ClockAndTimeFrame Decode(PodiumClockFrame baseFrame)
        {
            if (baseFrame == null) throw new ArgumentNullException(nameof(baseFrame));
            if (!baseFrame.Type.SequenceEqual(Type)) throw new Exception($"Wrong type of Frame {Type}");
            var clockAndTimeFrame = new ClockAndTimeFrame();

            var body = baseFrame.Body;


            var splitBySeparator = SplitBySeparator(body, clockAndTimeFrame);

            if (splitBySeparator.Count >= 4)
            {
                clockAndTimeFrame.Sign = Encoding.UTF8.GetChars(splitBySeparator[0]).First();
                clockAndTimeFrame.Minutes = Encoding.UTF8.GetString(splitBySeparator[0].Skip(1).ToArray());
                clockAndTimeFrame.Seconds = Encoding.UTF8.GetString(splitBySeparator[1]);
                clockAndTimeFrame.IndicatorEnabled = splitBySeparator[2].First() != (byte) 0;


                var textStart = splitBySeparator[3].First() == clockAndTimeFrame.TextStartBytes;
                
                var textEnd = splitBySeparator[3].Reverse().First() == clockAndTimeFrame.TextEndBytes;

                if (textStart && textEnd)
                {
                    var textBytes = splitBySeparator[3].Skip(1).Take(splitBySeparator[3].Length - 2).ToArray();

                    clockAndTimeFrame.Text = Encoding.UTF8.GetString(textBytes);
                }
            }

            return clockAndTimeFrame;
        }

        private static List<byte[]> SplitBySeparator(byte[] body, ClockAndTimeFrame clockAndTimeFrame)
        {
            List<byte[]> bytes = new List<byte[]>();
            List<byte> valuesBytes = new List<byte>();
            var textStarted = false;
            foreach (var byteValue in body)
            {
                if (byteValue == Encoding.UTF8.GetBytes(new[] {clockAndTimeFrame.Separator}).First() && !textStarted)
                {
                    bytes.Add(valuesBytes.ToArray());
                    valuesBytes.Clear();
                }
                else
                {
                    if (byteValue == clockAndTimeFrame.TextStartBytes)
                    {
                        textStarted = true;
                    }

                    if (byteValue == clockAndTimeFrame.TextEndBytes)
                    {
                        textStarted = false;
                    }

                    valuesBytes.Add(byteValue);
                }
            }

            bytes.Add(valuesBytes.ToArray());

            return bytes;
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