using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Exceptions
{
    public class NoConfigException : Exception
    {
        public NoConfigException(string message)
      : base(message)
        {
        }
    }
}
