using System;
using System.Collections.Generic;
using System.Text;

namespace ItSoft.ClientService
{
    class ClientService : IClockService
    {
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<ClockAndTextMessageEventArgs> ClockAndTextMessageReceived;
        private ISocketDataDecoder _dataDecoder;

        public ClientService(ISocketDataDecoder dataDecoder)
        {
            _dataDecoder = dataDecoder;
        }

        public void Start()
        {
            
            _dataDecoder.Connected += OnDataDecoderOnConnected;
            _dataDecoder.Disconnected += OnDataDecoderOnDisconnected;
            _dataDecoder.Start();

            _dataDecoder.FrameReceived += _dataDecoder_FrameReceived;


        }

        private void _dataDecoder_FrameReceived(object sender, ClockDataEventArgs<byte[]> e)
        {
            throw new NotImplementedException();
        }

        private void OnDataDecoderOnDisconnected(object sender, EventArgs args)
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void OnDataDecoderOnConnected(object sender, EventArgs args)
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            _dataDecoder.Connected += OnDataDecoderOnConnected;
            _dataDecoder.Disconnected += OnDataDecoderOnDisconnected;
            _dataDecoder.Stop();
        }
    }
}
