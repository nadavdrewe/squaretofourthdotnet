using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{

    public class Modifier : IRevelAddressable, IRevelCreateable
    {

        public bool active { get; set; }
        public string available_seats { get; set; }
        public string barcode { get; set; }
        public int color_code { get; set; }
        public string cost { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool deleted { get; set; }
        public string description { get; set; }
        public bool display_in_kiosk { get; set; }
        public bool display_online { get; set; }
        public bool do_not_print { get; set; }
        public string establishment { get; set; }
        public int id { get; set; }
        public string img_url { get; set; }
        public bool is_hot { get; set; }
        public bool is_quick { get; set; }
        public string kitchen_print_name { get; set; }
        public string modifierClass { get; set; }
        public string name { get; set; }
        public bool no_modifier_substitute { get; set; }
        public bool prep_recipe { get; set; }
        public string prep_yield { get; set; }
        public decimal price { get; set; }
        //  public List<string> printers { get; set; }
        public string resource_uri { get; set; }
        public string sku { get; set; }
        public int sort { get; set; }
        public bool substitute_modifiers { get; set; }
        // public List<string> substitutions { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string uuid { get; set; }
        [JsonIgnore]
        public string theAddress { get => "/resources/modifier?format=json&limit=0&active=1"; set => throw new NotImplementedException(); }

        public int Create(dynamic jsonModifier)
        {

            PropertyInfo[] properties = typeof(Modifier).GetProperties();
            foreach (PropertyInfo property in properties)
            {

                var currentProp = property.Name;
                try
                {
                    if (currentProp != "theAddress")
                    {

                        var type = property.PropertyType;

                        var propAsString = jsonModifier[property.Name];
                        var test = Convert.ChangeType(propAsString, type);

                        property.SetValue(this, test);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return 0;
        }
    }

}
