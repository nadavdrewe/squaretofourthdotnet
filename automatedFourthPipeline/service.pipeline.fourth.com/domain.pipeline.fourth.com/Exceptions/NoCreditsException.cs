using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Exceptions
{
    public class NoCreditsException : Exception
    {
        public NoCreditsException(string message)
      : base(message)
        {
        }
    }
}
