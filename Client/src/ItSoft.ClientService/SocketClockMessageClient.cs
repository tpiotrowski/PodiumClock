using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ItSoft.ClientService
{
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


        public int ReadPeriod { get; set; } = 100;
        public int WatchDogPeriod { get; set; } = 10000;

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

                var buffer = new byte[1024];

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