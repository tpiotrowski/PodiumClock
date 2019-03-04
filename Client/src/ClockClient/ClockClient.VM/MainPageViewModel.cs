using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Views;
using ItSoft.ClientService;
using ItSoft.Command;
using Xamarin.Forms;
using PropertyChangingEventArgs = System.ComponentModel.PropertyChangingEventArgs;
using PropertyChangingEventHandler = System.ComponentModel.PropertyChangingEventHandler;

namespace ClockClient.VM
{

    public class BaseView : INotifyPropertyChanged,INotifyPropertyChanging
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected bool Set<T>(
            ref T field,
            T newValue = default(T),
            bool broadcast = false,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            PropertyChanging?.Invoke(this,new PropertyChangingEventArgs(propertyName));

           
            field = newValue;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }
        
    }

    public class MainPageViewModel  : BaseView
    {
        private readonly INavigation _navigation;


        //ToDo: Add custom interface
        public MainPageViewModel(INavigation navigation,IMessagingCenter messagingCenter)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            
            
            messagingCenter.Subscribe<ClockAndTextMessage>(this,nameof(ClockAndTextMessage), msg =>
            {
                Time = msg.Time;
                Text = msg.Text;
                Indicator = msg.IndicatorEnabled;
            });

            messagingCenter.Subscribe<ClientConnectionStatus>(this, nameof(ClientConnectionStatus), msg =>
            {
                if (!msg.IsConnected)
                {
                    Time = "Brak połączenia.";
                    Text = String.Empty;
                    Indicator = false;
                }
            });


        }

        private bool _indicator;

        public bool Indicator
        {
            set => Set(ref _indicator, value);
            get => _indicator;
        }

        private string _text;

        public string Text
        {
            set => Set(ref _text, value);
            get => _text;
        }

        private string _time;

        public string Time
        {
            set => Set(ref _time, value);
            get => _time;
        }


    }

    //public interface ICommand
    //{
    //    void Execute(object arg);
    //    bool CanExecute(object arg);
    //    event EventHandler CanExecuteChanged; 
    //} 

   
}
