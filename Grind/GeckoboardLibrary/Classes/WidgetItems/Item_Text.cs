using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;

namespace GeckoboardLibrary.Classes.WidgetItems
{
    public class Item_Text :Item
    {

        public Item_Text()
        { }


        public Item_Text(string text, int type) : base(GeckoboardChartAndItemType.Text)
        {
            this.text = text;
            this.type = type;
        }
        public string text { get; set; }
        public int type { get; set; }


    }
}
