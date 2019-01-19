using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;

namespace ItSoft.ClientService
{
    public class BaseMessage
    {
        public static byte[] FrameStartBytes { get; set; } = {0x11};
        public static byte[] FrameEndBytes { get; set; } = {0x13};
        protected byte FrameTypeLength { get; set; } = 2;

        public byte[] Type { get; set; }
        public byte[] Body { get; set; }

        public static BaseMessage Decode(byte[] frame)
        {
            var podiumClockFrame = new BaseMessage();

            var startFrameExists = frame.Take(FrameStartBytes.Length)
                .SequenceEqual(FrameStartBytes);

            var frameEndExists = frame.Reverse().Take(FrameEndBytes.Length).Reverse()
                .SequenceEqual(FrameEndBytes);


            if (startFrameExists && frameEndExists)
            {
                var tmpFrame = frame.Skip(FrameStartBytes.Length)
                    .Take(frame.Length - FrameStartBytes.Length - FrameEndBytes.Length);


                podiumClockFrame.Type = tmpFrame.Take(podiumClockFrame.FrameTypeLength).ToArray();
                podiumClockFrame.Body = tmpFrame.Skip(podiumClockFrame.FrameTypeLength).ToArray();


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
}