using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes.WidgetItems;
using GeckoboardLibrary.Classes.Widgets;


namespace GeckoboardLibrary.Classes
{
    public class GeckoboardObjectCreatorFactory : IGeckoboardObjectCreatorFactory
    {
        public GeckoboardOrganisation GeckoboardOrganisation { get; set; }

        public GeckoboardObjectCreatorFactory(GeckoboardOrganisation geckoboardOrganisation)
        {
            //sets this so all widgets have access to org and api key
            this.GeckoboardOrganisation = geckoboardOrganisation;

        }

        

        public Line CreateLine(int id, string chartName, string pushURL, List<decimal> items, LineSettings settings)
        {
            try
            {
                Line line = new Line(id, this.GeckoboardOrganisation.api_key, pushURL, chartName, GeckoboardChartAndItemType.Line, settings);
                line.data.item = new List<decimal>();

                foreach (var item in items)
                {
                    line.data.item.Add(item);
                }

                //now add axis



                //now add settings

                return line;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public Bullet CreateBullet(int id,string chartName, string pushURL, string orientation, BulletItem item)
        {
            Bullet bullet = new Bullet(id,this.GeckoboardOrganisation.api_key, pushURL, chartName, GeckoboardChartAndItemType.Bullet);

            bullet.data.orientation = orientation;
            bullet.data.item = item;

            return bullet;

        }


        //Create NumberSecStatWidget
        public NumberSecondaryStat CreateNumberSecondaryStat(int id, string chartName, string pushURL, string firstStatName, int firstStatvalue, string comparisonName, int comparisonValue)
        {

            try
            {

                //new up
                NumberSecondaryStat numberSecondaryStatToReturn = new NumberSecondaryStat(id, this.GeckoboardOrganisation.api_key, pushURL, chartName, GeckoboardChartAndItemType.NumberSecondaryStat);

                //create correct data items
                Item_NumberSecondaryStat newItem = new Item_NumberSecondaryStat(firstStatName, firstStatvalue);
                Item_NumberSecondaryStat oldItem = new Item_NumberSecondaryStat(comparisonName, comparisonValue);

                //add data items to object
                numberSecondaryStatToReturn.data.item = new List<Item>(); //has already been initialised but wtf eh
                //return object
                numberSecondaryStatToReturn.data.item.Add(newItem);
                numberSecondaryStatToReturn.data.item.Add(oldItem);

                return numberSecondaryStatToReturn;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List CreateList(int id, string chartName, string pushURL, List<Item_List> items)
        {

            try
            {
                List list = new List(id, this.GeckoboardOrganisation.api_key, pushURL, chartName)
                {                               
                    type = GeckoboardChartAndItemType.List,
                    data = new ListData()
                };

                list.data.item = items;

                return list;
            }
            catch (Exception)
            {
                
                throw;
            }

        }


        public Text CreateText(int id, string chartName, string pushURL, List<Item_Text> theItems)
        {
            try
            {
                Text textToReturn = new Text(id, this.GeckoboardOrganisation.api_key, pushURL, chartName, GeckoboardChartAndItemType.Text);

                foreach (var item in theItems)
                {
                    textToReturn.data.item.Add(item);
                }

                return textToReturn;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        

    }
}
