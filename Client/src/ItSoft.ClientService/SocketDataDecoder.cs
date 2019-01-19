using System;
using System.Collections.Generic;
using System.Threading;

namespace ItSoft.ClientService
{
    public class SocketDataDecoder
    {
        private readonly IClockMessageClient<byte[]> _messageClient;

        public event EventHandler<ClockDataEventArgs<byte[]>> FrameReceived;
        private readonly List<byte> _bufferBytes = new List<byte>();
        private Timer _processDataTimer = null;
        public int ProcessingPeriod { get; set; } = 50;
        private readonly object _lockObject = new object();

        public SocketDataDecoder(IClockMessageClient<byte[]> messageClient)
        {
            _messageClient = messageClient;
            _messageClient.DataReceived += _messageClient_DataReceived;
            _messageClient.ClientConnected += _messageClient_ClientConnected;
            _messageClient.ClientDisconnected += _messageClient_ClientDisconnected;
        }

        private void _messageClient_ClientDisconnected(object sender, EventArgs e)
        {
            _processDataTimer?.Dispose();
            _processDataTimer = null;
        }

        private void _messageClient_ClientConnected(object sender, EventArgs e)
        {
            _processDataTimer = new Timer(state =>
            {
                if (state is List<byte> bag)
                    ProcessData(bag);

                _processDataTimer?.Change(ProcessingPeriod, Timeout.Infinite);
            }, _bufferBytes, 0, Timeout.Infinite);
        }

        private void ProcessData(List<byte> buffer)
        {
            lock (_lockObject)
            {
                var frameStartBytes = BaseMessage.FrameStartBytes;
                var frameEndBytes = BaseMessage.FrameEndBytes;

                var frameStartIndex = -1;
                var frameEndIndex = -1;
                var lastFrameEnd = -1;
                for (var i = 0; i < buffer.Count; i++)
                {
                    var startSequence = CheckStart(buffer, frameStartBytes, i);
                    if (startSequence > -1)
                    {
                        frameStartIndex = startSequence;
                    }

                    var sequence = CheckEnd(buffer, frameEndBytes, i);
                    if (sequence > -1)
                    {
                        frameEndIndex = sequence;
                    }

                    if (frameStartIndex > -1 && frameEndIndex > -1)
                    {
                        var length = frameEndIndex - frameStartIndex;
                        var range = buffer.GetRange(frameStartIndex, length).ToArray();
                        FrameReceived?.Invoke(this, new ClockDataEventArgs<byte[]>() {Message = range});
                        lastFrameEnd = frameEndIndex;
                        frameStartIndex = frameEndIndex = -1;
                    }
                }

                if(lastFrameEnd > -1)
                    buffer.RemoveRange(0, lastFrameEnd);
            }
        }

        private static int CheckStart(List<byte> buffer, byte[] frameStartBytes, int i)
        {
            
            var isFrameStart = true;
            var index = 0;
            for (; index < frameStartBytes.Length; index++)
            {
                var frameStartByte = frameStartBytes[index];
                isFrameStart = isFrameStart && frameStartByte == buffer[i];
                i++;
            }

            return isFrameStart ? i -1  : -1;
        }

        private static int CheckEnd(List<byte> buffer, byte[] frameStartBytes, int i)
        {

            var isFrameStart = true;
            var index = 0;
            for (; index < frameStartBytes.Length; index++)
            {
                var frameStartByte = frameStartBytes[index];
                isFrameStart = isFrameStart && frameStartByte == buffer[i];
                i++;
            }

            return isFrameStart ? i : -1;
        }

        private void _messageClient_DataReceived(object sender, ClockDataEventArgs<byte[]> e)
        {
            lock (_lockObject) _bufferBytes.AddRange(e.Message);
        }
    }
}