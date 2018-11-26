using System;
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
            
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }



    public class ViewModelLocator
    {
        public MainPageViewModel MainWindowModel
        {
            get { return new MainPageViewModel(); }
        }
    }
}
