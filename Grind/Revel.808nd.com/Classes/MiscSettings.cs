using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class MiscSettings
    {
        [Key]
        public int Id { get; set; }

        public bool LateOpeningStoreNotifier { get; set; }
        public int LateOpeningStoreMinutesWindow { get; set; }
    }
}