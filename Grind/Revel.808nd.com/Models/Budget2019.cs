using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Models
{
    public class Budget2019
    {
        public int Id { get; set; }
        public int EstablishmentId { get; set; }
        public DateTime BudgetDate { get; set; }
        public decimal Amount { get; set; }

    }
}
