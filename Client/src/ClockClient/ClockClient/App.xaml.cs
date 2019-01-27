using System;
using Android.Content.Res;
using ClockClient.Views;
using ClockClient.VM;
using ItSoft.ClientService.Di;
using Splat;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ClockClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            MainPage = (Current.Resources["ViewModelLocator"] as ViewModelLocator).MainPage;
            
            ClockClientServiceDependency.Configure(Locator.CurrentMutable, "192.168.1.6", 8811);
            
            
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
    }
}