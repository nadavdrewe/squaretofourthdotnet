using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Exceptions
{
    public class UnrecognisedSalesItemException : Exception
    {
        public UnrecognisedSalesItemException(string message) : base(message)
        {

        }
    }
}
