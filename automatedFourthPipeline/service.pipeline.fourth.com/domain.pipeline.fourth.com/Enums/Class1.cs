using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Enums
{
    public enum DataGatherResult
    {
        None,
        OrderEmpty,
        Complete,
        Error
    }

    public class DataGatherResultAndException
    {
        public DataGatherResult DataGatherResult { get; set; }
        public Exception Exception { get; set; }
    }
}
