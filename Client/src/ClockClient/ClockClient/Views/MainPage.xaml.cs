using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClockClient
{
    public partial class MainPage : ContentPage
    {
        //ScreenMetrics metrics = DeviceDisplay.Main;

        public MainPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasNavigationBar(this, false); 
            
       
        }

        private void HandleTapped(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MainPage_OnSizeChanged(object sender, EventArgs e)
        {
            //ToDo: Label autosize!!!
        }

        private void SwipeGestureRecognizer_OnSwiped(object sender, SwipedEventArgs e)
        {
            
        }
    }

}