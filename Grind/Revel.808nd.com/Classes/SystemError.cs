using System;

namespace Revel._808nd.com.Classes
{
    public class SystemError
    {
        public int id { get; set; }
        public Nullable<int> Establishment { get; set; }
        public Nullable<int> Brand { get; set; }
        public Nullable<int> ErrorCode { get; set; }
        public DateTime ErrorDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

       

    }
}
