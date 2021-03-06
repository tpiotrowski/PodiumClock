﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ItSoft.ClientService
{
    public class SocketClockMessageClient : IClockMessageClient<byte[]>
    {
        public event EventHandler<ClockDataEventArgs<byte[]>> DataReceived;
        public event EventHandler ClientDisconnected;
        public event EventHandler ClientConnected;
        private Timer _timer = null;
        private Timer _watchDog = null;

        protected bool _isStarted = false;

        private Socket _socket = null;

        private string _ipAddress;
        private int _port;
        public int ReadPeriod { get; set; } = 50;
        public int WatchDogPeriod { get; set; } = 10_000;
        public int BufferSize { get; set; } = 64_000;

        public SocketClockMessageClient(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void ChangeSettings(string ipAddress, int port)
        {
            StopClient();

            _ipAddress = ipAddress;
            _port = port;

            StartClient();
        }


        public void StartClient()
        {
            

            Connect();

            _isStarted = true;
        }

        public void StopClient()
        {
            _isStarted = false;

            _timer?.Dispose();
            _timer = null;

            _watchDog?.Dispose();
            _watchDog = null;

            _socket.Dispose();
            _socket = null;
        }

        bool SocketConnected(Socket s)
        {

            try
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
            catch(ObjectDisposedException)
            {
                return false;
            }

            
        }


        private void Connect()
        {
            var socketConnected = SocketConnected(_socket);

            if (socketConnected)
            {
                return;
            }

            var ipEndPoint = CreateSocket();

            
             var result = _socket.BeginConnect(ipEndPoint, res =>
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

        private IPEndPoint CreateSocket()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return ipEndPoint;
        }

        private void RunConnectionWatchDog()
        {
            if (!_isStarted) return;

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
            if (!_isStarted) return;

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