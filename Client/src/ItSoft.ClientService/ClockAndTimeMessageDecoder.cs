using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoft.ClientService;

namespace ItSoft.ClientService
{
    public class ClockAndTimeMessageDecoder
    {
        public static readonly byte[] Type = {Convert.ToByte('T'), Convert.ToByte('1')};
        protected byte TextStartBytes { get; set; } = 0x2;
        protected byte TextEndBytes { get; set; } = 0x3;      
        protected char Separator { get; set; } = ':';
        
        public static ClockAndTextMessage Decode(BaseMessage baseFrame)
        {
            if (baseFrame == null) throw new ArgumentNullException(nameof(baseFrame));
            if (!baseFrame.Type.SequenceEqual(Type)) throw new Exception($"Wrong type of Frame {Type}");
            var clockAndTimeFrame = new ClockAndTimeMessageDecoder();

            var body = baseFrame.Body;


            var splitBySeparator = SplitBySeparator(body, clockAndTimeFrame);



            ClockAndTextMessage msg = new ClockAndTextMessage();
            if (splitBySeparator.Count >= 4)
            {
                msg.Sign = Encoding.UTF8.GetChars(splitBySeparator[0]).First();
                msg.Minutes = Encoding.UTF8.GetString(splitBySeparator[0].Skip(1).ToArray());
                msg.Seconds = Encoding.UTF8.GetString(splitBySeparator[1]);
                msg.IndicatorEnabled = splitBySeparator[2].First() != (byte) 0x30;


                var textStart = splitBySeparator[3].First() == clockAndTimeFrame.TextStartBytes;
                
                var textEnd = splitBySeparator[3].Reverse().First() == clockAndTimeFrame.TextEndBytes;

                if (textStart && textEnd)
                {
                    var textBytes = splitBySeparator[3].Skip(1).Take(splitBySeparator[3].Length - 2).ToArray();

                    msg.Text = Encoding.UTF8.GetString(textBytes);
                }
            }

            return msg;
        }

        private static List<byte[]> SplitBySeparator(byte[] body, ClockAndTimeMessageDecoder clockAndTimeMessage)
        {
            List<byte[]> bytes = new List<byte[]>();
            List<byte> valuesBytes = new List<byte>();
            var textStarted = false;
            foreach (var byteValue in body)
            {
                if (byteValue == Encoding.UTF8.GetBytes(new[] {clockAndTimeMessage.Separator}).First() && !textStarted)
                {
                    bytes.Add(valuesBytes.ToArray());
                    valuesBytes.Clear();
                }
                else
                {
                    if (byteValue == clockAndTimeMessage.TextStartBytes)
                    {
                        textStarted = true;
                    }

                    if (byteValue == clockAndTimeMessage.TextEndBytes)
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
}