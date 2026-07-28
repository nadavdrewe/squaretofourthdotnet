using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;

namespace Geckoboard._808nd.com
{



    public class Bullet : GeckoboardObject
    {
        public BulletData data { get; set; }


        public Bullet()
        {
            this.data = new BulletData();
        }

        public Bullet(int id, string api_key, string pushURL, string chartName, GeckoboardChartAndItemType type)
        {
            //anything else?
            this.ID = id;
            this.data = new BulletData();
            this.chartName = chartName;
            this.api_key = api_key;
            this.pushURL = pushURL;
            this.type = type;
            this.data.item = new BulletItem();

        }

    }

    public class BulletData
    {
        public string orientation { get; set; }
        public BulletItem item { get; set; }

    }

    public class BulletItem
    {
        public string label { get; set; }
        public string sublabel { get; set; }

        public BulletAxis axis { get; set; }
        public List<BulletRange> range { get; set; }
        public BulletMeasure measure { get; set; }       
        public BulletComparative comparative { get; set; }

    }

    public class BulletAxis
    {
        public List<string> point { get; set; }

    }


    public class BulletRange
    {

        public string color { get; set; }
        public int start { get; set; }
        public int end { get; set; }

    }

    public class BulletMeasure
    {
        public BulletMeasureItem current { get; set; }
        public BulletMeasureItem projected { get; set; }
    }

    public class BulletMeasureItem
    {
        public string start { get; set; }
        public string end { get; set; }

    }

    public class BulletComparative
    {

        public string point { get; set; }
    }


}
