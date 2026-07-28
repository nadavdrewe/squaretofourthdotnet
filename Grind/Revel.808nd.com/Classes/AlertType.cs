using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class AlertType
    {
        [Key]
        public int AlertTypeID { get; set; }
        public string Typename { get; set; }

        

    }
}
