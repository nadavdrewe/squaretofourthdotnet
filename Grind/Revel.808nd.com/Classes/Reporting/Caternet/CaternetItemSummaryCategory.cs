using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting.Caternet
{
    public class CaternetItemSummaryParentCategory : CaternetItemSummaryCategory
    {
        public int ParentCategoryId;
        public string ParentCategoryName { get; set; }
        public List<CaternetItemSummaryCategory> Categories { get; set; }

    }

    public class CaternetItemSummaryCategory
    {
        public int Id;
        public string Name { get; set; }
        public List<CaternetItemSummary> Summaries { get; set; }

    }
}
