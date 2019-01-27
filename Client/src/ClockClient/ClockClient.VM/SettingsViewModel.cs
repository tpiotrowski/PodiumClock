namespace ClockClient.VM
{
    public class SettingsViewModel : BaseView
    {
        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => Set(ref _ipAddress ,value);
        }

        private int _portNumber;

        public int PortNumber
        {
            get => _portNumber;
            set => Set(ref _portNumber, value);
        }
    }
}