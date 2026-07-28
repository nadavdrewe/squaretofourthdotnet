using System.Collections.Generic;
using WebReboot.Grind._808nd.com.ViewModels;

namespace WebReboot.Grind._808nd.com.Models.ViewModels
{
    public class CardLogPartialModel
    {
        public IList<LogViewModel> Logs { get; set; }
        public IList<OrderItemViewModel> OrderItems { get; set; }
        public IList<ProductViewModel> Products { get; set; }
        public IList<EstablishmentViewModel> Establishments { get; set; }
    }
}