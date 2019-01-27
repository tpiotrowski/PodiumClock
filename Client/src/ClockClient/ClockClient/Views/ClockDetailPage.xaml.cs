using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClockClient.VM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClockDetailPage : ContentPage
    {
        public ClockDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            var mainPageViewModel = BindingContext as MainPageViewModel;


            if (mainPageViewModel != null)
            {
                mainPageViewModel.PropertyChanged += MainPageViewModel_PropertyChanged;
            }

            base.OnBindingContextChanged();
        }

        private void MainPageViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainPageViewModel.Indicator))
            {
                var model = sender as MainPageViewModel;

                var timeLabelTextColor = _timeLabel.TextColor;


                Device.BeginInvokeOnMainThread(async () =>
                {
                    while (model.Indicator)
                    {
                        _timeLabel.TextColor = Color.OrangeRed;

                        await Task.WhenAll(
                            //_timeLabel.FadeTo(1, 1000),
                            _timeViewBox.ScaleTo(1.2, 500)
                           );


                        await Task.WhenAll(
                            //_timeLabel.FadeTo(1, 1000),
                            _timeViewBox.ScaleTo(1, 500)
                            );
                        _timeLabel.TextColor = timeLabelTextColor;
                    }
                });
            }
        }
    }
}