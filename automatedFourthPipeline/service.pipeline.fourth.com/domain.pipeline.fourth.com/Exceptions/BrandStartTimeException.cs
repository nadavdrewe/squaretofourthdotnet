using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Exceptions
{
    
    public class BrandStartTimeException : Exception
    {
        public BrandStartTimeException(string message)
      : base(message)
        {


        }

        public BrandStartTimeException(string message, Exception innerExcption)
     : base(message, innerExcption)
        {
            
        }
    }
}
