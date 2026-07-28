using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.SalesFactories.Helper
{

    public class RecordActivityCodeService
    {
        public int _currentCount = 0;

        public RecordActivityCodeService()
        {

        }

        public void Increment()
        {
            _currentCount = ++_currentCount;
        }

        public void ResetToZero()
        {
            _currentCount = 0;
        }

        public string GetCurrentActivityCode()
        {
            return _currentCount.ToString();
        }
    }

}
