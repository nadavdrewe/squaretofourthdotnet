using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{

    public class MenuFile
    {
        [Key]
        public int id { get; set; }
        public byte[] bytes { get; set; }
        public string filename { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
        public virtual Menu Menu { get; set; }
    }
}