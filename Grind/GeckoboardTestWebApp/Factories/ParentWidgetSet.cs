using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeckoboardTestWebApp.Models
{
    public class ParentWidgetSet : WidgetSetA
    {
        public List<WidgetSetA> AllChildWidgetSets { get; set; }

        public ParentWidgetSet(List<WidgetSetA> theChildWidgetSets)
        {
            this.AllChildWidgetSets = theChildWidgetSets;
        }

        public ParentWidgetSet()
        {
        }



    }
}