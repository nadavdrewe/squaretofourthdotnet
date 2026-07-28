using System;
using System.Collections.Generic;
using System.Text;

namespace shared.pipeline.fouth.com.Enums
{
    public enum CredentialTypes
    {
        /// <summary>
        /// This is a default value
        /// </summary>
        None = 0,

        /// <summary>
        /// This is the standard Revel API 
        /// </summary>
        RevelApi = 100,

        /// <summary>
        /// Lightspeed Restaurant
        /// </summary>
        LightspeedRestoApi = 130,

        /// <summary>
        /// For Square PoS
        /// </summary>
        SquareApi = 110,


        /// <summary>
        /// For Fourth Hospitality
        /// </summary>
        FourthBaseCredential = 120
    }
}
