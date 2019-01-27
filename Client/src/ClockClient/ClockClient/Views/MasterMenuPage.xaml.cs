using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClockClient.VM;
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

        class MainMasterDetailMasterViewModel : BaseView
        {
            private ObservableCollection<MainMasterDetailMenuItem> _menuItems;
            private MainMasterDetailMenuItem _selectedMenuItem;

            public ObservableCollection<MainMasterDetailMenuItem> MenuItems
            {
                get => _menuItems;
                set => Set(ref _menuItems,value);
            }


            public MainMasterDetailMenuItem SelectedMenuItem
            {
                get => _selectedMenuItem;
                set => Set(ref _selectedMenuItem,value);
            }

            public MainMasterDetailMasterViewModel()
            {
                //Todo: move this to ViewModel   
                var mainMasterDetailMenuItem = new MainMasterDetailMenuItem { Id = 0, Title = "Zegar",Image = "clock.png", TargetType = typeof(ClockDetailPage)};
                MenuItems = new ObservableCollection<MainMasterDetailMenuItem>(new[]
                {
                    mainMasterDetailMenuItem,
              
                });

                SelectedMenuItem = mainMasterDetailMenuItem;
            }

        }
    }
}