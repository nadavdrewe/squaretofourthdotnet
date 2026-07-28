using GeckoboardLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geckoboard._808nd.com
{
    public class List : GeckoboardObject
    {
        public ListData data { get; set; }

        public List()
        {
            this.data = new ListData();
        }

        public List(int id, string api_key, string pushURL, string chartName)
        {
            this.api_key = api_key;
            this.pushURL = pushURL;
            this.chartName = chartName;
        }

    }

    public class ListData
    {

        public List<Item_List> item {get; set;}

        public ListData()
        {
            this.item = new List<Item_List>();
        }
        
    }

    public class Item_List
    {
        public Label_List label { get; set; }
        public Title_List title { get; set; }
        public string description { get; set; }

        public Item_List()
        {
            this.label = new Label_List();
            this.title = new Title_List();

        }

    }

    public class Label_List
    {
        public string name { get; set; }
        public string color {get;set;}

    }

    public class Title_List
    {
        public string text { get; set; }
        public bool highlight { get; set; }

    }
}
