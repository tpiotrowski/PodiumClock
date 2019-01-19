using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ClockClient.Android.Services;
using Xamarin.Forms;

namespace ClockClient.Droid
{
    [Activity(Label = "ClockClient", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

           

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            MessagingCenter.Subscribe<StartClockClientTask>(this, nameof(StartClockClientTask), message => {
                var intent = new Intent(this, typeof(ClockClientService));
                StartService(intent);
            });

            MessagingCenter.Subscribe<StopClockClientTask>(this, nameof(StopClockClientTask), message =>
            {
                var intent = new Intent(this, typeof(ClockClientService));
                StopService(intent);
            });
        }


      
    }
}