using System;
using System.Collections.Generic;
using System.Text;

namespace com.fourth.pipeline.pos.Enum
{
    /// <summary>
    /// Max 2 breaks per shift in Fourth!!
    /// </summary>
    public enum TimesheetClockStatus
    {
        ClockOut = 0,
        ClockIn =1 ,
        BreakStart = 3,
        BreakEnd = 4
    }
}
