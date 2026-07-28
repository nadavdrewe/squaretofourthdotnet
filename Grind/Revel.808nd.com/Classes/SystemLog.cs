using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class SystemLog
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string WhoTriggered { get; set; }
        public DateTime WhenCreated { get; set; }


    }
}
