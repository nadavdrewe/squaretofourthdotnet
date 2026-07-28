using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;

namespace GeckoboardLibrary.Classes.WidgetItems
{
    public class Leaderboard_Item : Item
    {
        public string label { get; set; }
        public int value { get; set; }
        public int previous_rank { get; set; }

        public Leaderboard_Item(string Label, int Value, int Previousrank) : base((GeckoboardChartAndItemType.Leaderboard))
        {
            this.label = Label;
            this.value = Value;
            this.previous_rank = Previousrank;
        }

    }


}
