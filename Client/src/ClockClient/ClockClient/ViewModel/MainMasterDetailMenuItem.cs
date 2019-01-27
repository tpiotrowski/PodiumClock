using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockClient.Views
{

    public class MainMasterDetailMenuItem
    {
        public MainMasterDetailMenuItem()
        {
            TargetType = typeof(ClockDetailPage);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public Type TargetType { get; set; }
    }
}