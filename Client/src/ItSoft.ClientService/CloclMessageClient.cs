using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ItSoft.ClientService
{
    public class SocketClockMessageClient : IClockMessageClient<byte[]>
    {
        public event EventHandler<ClockDataEventArgs<byte[]>> DataReceived;
        private Timer _timer = null;
        private Timer _watchDog = null;
        private Socket _socket = null;
        private IPHostEntry _ipHostInfo;
        private IPAddress _address;
        private IPEndPoint _endPoint;

        public int ReadPeriod { get; set; } = 1000;
        public int WatchDogPeriod { get; set; } = 1000;

        public SocketClockMessageClient(string ipAddress, int port)
        {
            _ipHostInfo = Dns.GetHostEntry(ipAddress);
            _address = _ipHostInfo.AddressList.First();
            _endPoint = new IPEndPoint(_address, port);
        }

        private void Connect()
        {
            if (_socket == null)
            {
                _socket = new Socket(_address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }

            if (_socket.Connected) return;

            _socket.BeginConnect(_endPoint, res =>
            {
                var socket = (Socket) res.AsyncState;
                if (socket.Connected)
                {
                    _watchDog?.Dispose();
                    _timer = new Timer(ReadData, _socket, 0, WatchDogPeriod);                  
                }
                else
                {
                    _timer.Dispose();
                    _watchDog = new Timer(state => { Connect(); }, _socket, 0, WatchDogPeriod);
                }
            }, _socket);
        }

        private void ReadData(object state)
        {
            if (state is Socket socket)
            {
                try
                {
                }
                catch (Exception e)
                {
                    //Todo: Add logging abstraction
                }
            }
        }
    }
}