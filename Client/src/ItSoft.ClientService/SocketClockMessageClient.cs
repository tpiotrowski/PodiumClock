using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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


    public class SocketClockMessageClient : IClockMessageClient<byte[]>
    {
        private readonly string _ipAddress;
        private readonly int _port;
        public event EventHandler<ClockDataEventArgs<byte[]>> DataReceived;
        public event EventHandler ClientDisconnected;
        public event EventHandler ClientConnected;
        private Timer _timer = null;
        private Timer _watchDog = null;

        private Socket _socket = null;


        public int ReadPeriod { get; set; } = 50;
        public int WatchDogPeriod { get; set; } = 10_000;
        public int BufferSize { get; set; } = 64_000;

        public SocketClockMessageClient(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void StartClient()
        {
            Connect();
        }

        bool SocketConnected(Socket s)
        {
            if (s == null) return false;

            if (!s.Connected)
            {
                return false;
            }
            else
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }
        }


        private void Connect()
        {
            var socketConnected = SocketConnected(_socket);

            if (socketConnected)
            {
                return;
            }

            var ipEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(ipEndPoint, res =>
            {
                var socket = (Socket) res.AsyncState;

                if (SocketConnected(socket))
                {
                    ClientConnected?.Invoke(this, EventArgs.Empty);
                    _watchDog?.Dispose();
                    _watchDog = null;


                    _timer = new Timer(state =>
                    {
                        ReadData(state);

                        _timer?.Change(ReadPeriod, Timeout.Infinite);
                    }, _socket, 0, Timeout.Infinite);
                }
                else
                {
                    RunConnectionWatchDog();
                }
            }, _socket);
        }

        private void RunConnectionWatchDog()
        {
            ClientDisconnected?.Invoke(this, EventArgs.Empty);

            _timer?.Dispose();
            _timer = null;
            if (_watchDog == null)
                _watchDog = new Timer(state =>
                {
                    Connect();

                    _watchDog?.Change(WatchDogPeriod, Timeout.Infinite);
                }, null, 0, Timeout.Infinite);
        }

        private void ReadData(object state)
        {
            if (state is Socket socket)
            {
                if (!SocketConnected(socket))
                {
                    RunConnectionWatchDog();
                    return;
                }

                var buffer = new byte[BufferSize];

                try
                {
                    var byteReceive = _socket.Receive(buffer);

                    if (byteReceive > 0)
                    {
                        var message = new byte[byteReceive];

                        Array.Copy(buffer, message, byteReceive);

                        DataReceived?.Invoke(this, new ClockDataEventArgs<byte[]>() {Message = message});
                    }
                }
                catch (Exception e)
                {
                    //Todo: Add logging abstraction
                }
            }
        }
    }
}