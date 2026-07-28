using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Taxes;

namespace xero.railgunit.com.Grind
{
    public class RevelProductClassAccountMapping
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string AccountCode { get; set; }

    }

    public class RevelProductClassAccountMappingService
    {
        public RevelProductClassAccountMapping GetRevelAccountCodeForCategory(string revelCategory)
        {
            switch (revelCategory.Trim().ToLower())
            {
                //case "gift":
                //    return new RevelProductClassAccountMapping { CategoryName = "gift", AccountCode = "209" };
                case "unknown class":
                    return new RevelProductClassAccountMapping { CategoryName = "unknown class", AccountCode = "208" };
                case "extra items":
                    return new RevelProductClassAccountMapping { CategoryName = "extra items", AccountCode = "208" };
                case "retail":
                    return new RevelProductClassAccountMapping { CategoryName = "retail", AccountCode = "209" };
                case "bar":
                    return new RevelProductClassAccountMapping { CategoryName = "bar", AccountCode = "204" };
                case "coffee/hot drinks":
                    return new RevelProductClassAccountMapping { CategoryName = "coffee/hot drinks", AccountCode = "200" };
                case "events":
                    return new RevelProductClassAccountMapping { CategoryName = "events", AccountCode = "209" };
                case "food":
                    return new RevelProductClassAccountMapping { CategoryName = "food", AccountCode = "202" };
                case "soft drinks":
                    return new RevelProductClassAccountMapping { CategoryName = "soft drinks", AccountCode = "204" };
                case "drinks":
                    return new RevelProductClassAccountMapping { CategoryName = "drinks", AccountCode = "204" };
                case "juice":
                    return new RevelProductClassAccountMapping { CategoryName = "juice", AccountCode = "204" };
                case "discounts":
                    return new RevelProductClassAccountMapping { CategoryName = "discounts", AccountCode = "301" };

                //self defined categories for other items

                case "tips":
                    return new RevelProductClassAccountMapping { CategoryName = "tips", AccountCode = "413" };
                case "gift":
                    return new RevelProductClassAccountMapping { CategoryName = "gift", AccountCode = "810" };


                default:
                    throw new Exception("Couldnt' idenfity category string passed in to map account code");




            }
        }
    }
}