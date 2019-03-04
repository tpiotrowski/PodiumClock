using System.Windows.Input;
using ItSoft.Command;
using Xamarin.Forms;

namespace ClockClient.VM
{
    public class SettingsViewModel : BaseView
    {
        public static string SettingsChangedMessageIs = "77182AA6-C510-459A-8733-5425AF08A676";
        


        public SettingsViewModel()
        {
            PropertyChanged += SettingsViewModel_PropertyChanged;

            SaveCommand =
                new NativeCommand((o) =>
                    {
                        MessagingCenter.Send<SettingsViewModel>(this, SettingsChangedMessageIs);
                        IsDirty = false;
                    },
                    o => true);

            IsDirty = false;
        }

        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set {
                if (Set(ref _ipAddress, value))
                {
                    IsDirty = true;
                }
            }
        }

        private string _portNumber;
        private  bool _isDirty;

        public string PortNumber
        {
            get => _portNumber;
            set
            {
                if (Set(ref _portNumber, value))
                {
                    IsDirty = true;
                }
            }
        }

        public ICommand SaveCommand { get; set; }

        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty,value);
        }
    }
}