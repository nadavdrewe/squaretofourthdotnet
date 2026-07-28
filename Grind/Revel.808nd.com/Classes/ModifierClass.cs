using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class ModifierClass : IRevelAddressable, IRevelCreateable
    {
        public bool active { get; set; }
        public string admin_mod_key { get; set; }
        public bool admin_modifier { get; set; }
        public int color_code { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string establishment { get; set; }
        public int free_modifier { get; set; }
        public int id { get; set; }
        public int lock_add_modifier { get; set; }
        public string name { get; set; }
        public string resource_uri { get; set; }
        public int sort { get; set; }
        public bool split { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string theAddress { get => "/resources/modifierclass?format=json&limit=0&active=1"; set => throw new NotImplementedException(); }

        public int Create(dynamic jsonModifierClass)
        {
            PropertyInfo[] properties = typeof(ModifierClass).GetProperties();
            foreach (PropertyInfo property in properties)
            {

                var currentProp = property.Name;
                try
                {
                    if (currentProp != "theAddress")
                    {

                        var type = property.PropertyType;

                        var propAsString = jsonModifierClass[property.Name];
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

