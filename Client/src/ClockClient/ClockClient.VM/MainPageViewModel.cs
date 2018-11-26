using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Views;
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
        public ICommand OpenSettingsCommand { get;  }


        //ToDo: Add custom interface
        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            Text = "Text from view model";


            OpenSettingsCommand = (ICommand) new NativeCommandFactory().CreateInstance(obj =>
                {
                    Text = "Command was executed";
                }, o => true);

        }
        


        private string _text;

        public string Text
        {
            set
            {
                _text = value;
                Set(ref _text, value);
            }
            get => _text;
        }


    }

    public interface ICommand
    {
        void Execute(object arg);
        bool CanExecute(object arg);
        event EventHandler CanExecuteChanged; 
    } 

    public interface ICommandFactory
    {
        object CreateInstance(Action<object> action, Func<object, bool> predicate);
    }

    public class NativeCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _predicate;

        public NativeCommand(Action<object> action, Func<object, bool> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class NativeCommandFactory : ICommandFactory
    {
        public object CreateInstance(Action<object> action, Func<object, bool> predicate)
        {
            return new NativeCommand(action, predicate);
        }
    }
    
}
