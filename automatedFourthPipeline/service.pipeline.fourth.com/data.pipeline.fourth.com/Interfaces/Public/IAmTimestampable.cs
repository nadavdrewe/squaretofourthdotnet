using System;
using System.Collections.Generic;
using System.Text;

namespace data.pipeline.fourth.com.Interfaces.Public
{
    public interface IAmTimestampable
    {
        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
