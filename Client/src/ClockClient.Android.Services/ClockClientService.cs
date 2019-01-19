using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace ClockClient.Android.Services
{
    [Service(IsolatedProcess = true)]
    public class ClockClientService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        
        //Service is created
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        //Stop servie is called 
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
