using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterMenuPage : ContentPage
    {
        public ListView ListView;
        public ListView SettingsLV;

        public MasterMenuPage()
        {
            InitializeComponent();

            BindingContext = new MainMasterDetailMasterViewModel();
            ListView = MenuItemsListView;
            SettingsLV = SettingsListView;
        }

        class MainMasterDetailMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainMasterDetailMenuItem> MenuItems { get; set; }
            
            public MainMasterDetailMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainMasterDetailMenuItem>(new[]
                {
                    new MainMasterDetailMenuItem { Id = 0, Title = "Zegar",Image = "clock.png", TargetType = typeof(ClockDetailPage)},
              
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}