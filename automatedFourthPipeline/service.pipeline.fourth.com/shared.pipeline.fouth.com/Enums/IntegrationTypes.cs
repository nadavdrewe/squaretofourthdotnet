using System;
using System.Collections.Generic;
using System.Text;

namespace shared.pipeline.fourth.com

{
    public enum IntegrationTypes
    {
        None = 0,

        SquareToFourthPosSales = 100,
        RevelToFourthPosSales = 101,
        LightspeedRestoToFourthPosSales = 102,

        LighspeedToFourthTandA = 200,
        RevelToFourthTandA = 201
    }

    public enum IntegrationSubTypes
    {
        None = 0,
    }
}
