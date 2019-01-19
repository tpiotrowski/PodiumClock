using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace ClockClient.Android.Services
{
    public class ClockClientService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}
