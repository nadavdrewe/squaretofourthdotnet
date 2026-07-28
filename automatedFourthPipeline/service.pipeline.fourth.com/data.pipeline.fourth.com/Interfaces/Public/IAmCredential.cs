using data.pipeline.fourth.com.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using shared.pipeline.fouth.com.Enums;

namespace data.pipeline.fourth.com.Interfaces.Public
{
    public interface IAmCredential
    {
        /// <summary>
        /// What specific system does it belong to? 
        /// </summary>
        public CredentialTypes CredentialType { get; set; }
    }
}
