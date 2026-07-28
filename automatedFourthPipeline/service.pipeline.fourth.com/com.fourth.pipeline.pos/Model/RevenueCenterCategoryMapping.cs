using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos.Model
{
    /// <summary>
    /// Might be by store or might be by string / ID
    /// </summary>
    public class RevenueCenterCategoryMapping
    {
        public int Id { get; set; }


        public string CategoryName { get; set; }
        public string CategoryId { get; set; }

        
    }
}
