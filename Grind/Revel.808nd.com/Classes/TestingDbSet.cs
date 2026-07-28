using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public partial class TestingDbSet
    {
        [Key]
        public int id { get; set; }

        public string testAtt {get;set;}
    }
}
