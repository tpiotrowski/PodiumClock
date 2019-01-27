using System;
using Android.Content.Res;
using ClockClient.Views;
using ClockClient.VM;
using ItSoft.ClientService;
using ItSoft.ClientService.Di;
using Splat;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ClockClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            MainPage = (Current.Resources["ViewModelLocator"] as ViewModelLocator).MainPage;
            
            ClockClientServiceDependency.Configure(Locator.CurrentMutable, "10.0.2.2", 8811);
            Locator.CurrentMutable.Register<SettingsViewModel>( () => new SettingsViewModel());

            MessagingCenter.Subscribe<SettingsViewModel>(this, SettingsViewModel.SettingsChangedMessageIs, src =>
            {
                Preferences.Set("IpAddress", src.IpAddress);
                Preferences.Set("PortNumber",src.PortNumber);

                var clockMessageClient = Locator.CurrentMutable.GetService<IClockMessageClient<byte[]>>();

                clockMessageClient.ChangeSettings(src.IpAddress,Convert.ToInt32(src.PortNumber));

            });
        }

        protected override void OnStart()
        {
            MessagingCenter.Send(new StartClockClientTask(), nameof(StartClockClientTask));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            MessagingCenter.Send(new StopClockClientTask(), nameof(StopClockClientTask));
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            MessagingCenter.Send(new StartClockClientTask(), nameof(StartClockClientTask));
        }
    }


    public class ViewModelLocator
    {
        protected INavigation navigation;

        public ViewModelLocator()
        {
            var mainPage = new MainMasterDetail();

            MainPage = mainPage;
            navigation = MainPage.Navigation;

            var context = new MainPageViewModel(navigation,MessagingCenter.Instance);

            mainPage.BindingContext = context;
        }

        public MasterDetailPage MainPage { get; }


        public SettingsViewModel SettingsViewModel
        {
            get
            {
                var settingsViewModel = Locator.CurrentMutable.GetService<SettingsViewModel>();
                
                settingsViewModel.IpAddress = Preferences.Get("IpAddress", "10.0.2.2");
                settingsViewModel.PortNumber = Preferences.Get("PortNumber", "8811");

                settingsViewModel.IsDirty = false;

                return settingsViewModel;
            } 
        }
    }
}