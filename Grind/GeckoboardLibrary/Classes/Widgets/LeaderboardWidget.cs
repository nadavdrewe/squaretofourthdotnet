using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes.WidgetItems;

namespace GeckoboardLibrary.Classes.Widgets
{
    public class LeaderboardWidget : GeckoboardObject
    {

        public LeaderboardData data { get; set; }


        public LeaderboardWidget()
        {
            Init();
        }

        public LeaderboardWidget(string _pushURL)
        {

            Init();
            this.pushURL = _pushURL;
        }



        public void Init()
        {
            data = new LeaderboardData();
            data.items = new List<Leaderboard_Item>();
            this.type = GeckoboardChartAndItemType.Leaderboard;
        }

    }

    public class LeaderboardData
    {

        public string format = "currency";
        public string unit = "GBP";
        public IList<Leaderboard_Item> items { get; set; }
    }


}
