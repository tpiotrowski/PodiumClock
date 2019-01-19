using System;
using Android.App;
using Android.Content;
using Android.OS;
using ItSoft.ClientService;
using Splat;
using Xamarin.Forms;

namespace ClockClient.Android.Services
{
    [Service]
    public class ClockClientService : Service
    {
        private ISocketDataDecoder _dataDecoder;
        private IClockMessageClient<byte[]> _socketclockMessageClient;
        private IClockAndTimeMessageDecoder _clockAndTimeMessageDecoder;


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        //Service is created
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _socketclockMessageClient = Locator.CurrentMutable.GetService<IClockMessageClient<byte[]>>();
            _dataDecoder = Locator.CurrentMutable.GetService<ISocketDataDecoder>();
            _clockAndTimeMessageDecoder = Locator.CurrentMutable.GetService<IClockAndTimeMessageDecoder>();

            MessagingCenter.Subscribe<SocketDataSettings>(this,"", sds =>
            {
                _socketclockMessageClient.ChangeSettings(sds.Address,sds.Port);
            });

            _dataDecoder.FrameReceived += DataDecoder_FrameReceived;
            _dataDecoder.Connected += DataDecoder_Connected;
            _dataDecoder.Disconnected += DataDecoderOnDisconnected;
            _dataDecoder.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        private void DataDecoderOnDisconnected(object sender, EventArgs e)
        {
            MessagingCenter.Send(new ClientConnectionStatus()
            {
                IsConnected = false
            }, nameof(ClientConnectionStatus));
        }

        private void DataDecoder_Connected(object sender, EventArgs e)
        {
            MessagingCenter.Send(new ClientConnectionStatus()
            {
                IsConnected = true
            }, nameof(ClientConnectionStatus));
        }

        private void DataDecoder_FrameReceived(object sender, ClockDataEventArgs<byte[]> e)
        {
            var baseMessage = BaseMessage.Decode(e.Message);
            var clockAndTextMessage = _clockAndTimeMessageDecoder.Decode(baseMessage.Type, baseMessage.Body);

            MessagingCenter.Send(clockAndTextMessage, nameof(ClockAndTextMessage));
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        //Stop servie is called 
        public override void OnDestroy()
        {
            _dataDecoder.FrameReceived -= DataDecoder_FrameReceived;
            _dataDecoder.Connected -= DataDecoder_Connected;
            _dataDecoder.Disconnected -= DataDecoderOnDisconnected;
            _dataDecoder.Stop();
            base.OnDestroy();
        }
    }
}