using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClockClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMasterDetail : MasterDetailPage
    {
        public MainMasterDetail()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            MasterPage.SettingsLV.ItemSelected += ListViewSettingsOnItemSelected;
            
        }

        private void ListViewSettingsOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MasterPage.SettingsLV.SelectedItem = null;

            if (NavigateTo(e)) return;

            MasterPage.ListView.SelectedItem = null;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            MasterPage.SettingsLV.SelectedItem = null;

            if (NavigateTo(e)) return;

            MasterPage.ListView.SelectedItem = null;
        }

        private bool NavigateTo(SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainMasterDetailMenuItem;
            if (item == null)
                return true;

            var page = (Page) Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;
            return false;
        }
    }
}