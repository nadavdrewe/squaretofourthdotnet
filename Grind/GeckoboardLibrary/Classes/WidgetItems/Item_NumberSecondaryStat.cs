using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;

namespace GeckoboardLibrary.Classes.WidgetItems
{
    public class Item_NumberSecondaryStat : Item
    {

        public Item_NumberSecondaryStat() : base(GeckoboardChartAndItemType.NumberSecondaryStat)
        {
        }

        public Item_NumberSecondaryStat(string text, int value, string prefix = "")
            : base(GeckoboardChartAndItemType.NumberSecondaryStat)
        {
            this.text = text;
            this.value = value;
        }

        public string text { get; set; }
        public int value { get; set; }
      //  public string prefix { get; set; }

    }
}
