using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public MainPageViewModel()
        {
            Text = "Text from view model";
       
        }

    }
}
