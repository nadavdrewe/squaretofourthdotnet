using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }


    }
}
