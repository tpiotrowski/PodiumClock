using System;
using Android.Content.Res;
using ClockClient.VM;
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
            var mainPage = new MainPage();
            
            MainPage = new NavigationPage(mainPage);
            navigation = MainPage.Navigation;

            var context = new MainPageViewModel(navigation);

            mainPage.BindingContext = context;

            
        }

        public NavigationPage MainPage { get; }
    }
}
