using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Models
{
    public class BudgetContext : DbContext
    {
        static BudgetContext()
        {
            Database.SetInitializer<BudgetContext>(null);

        }

        public BudgetContext() : base("Name=BudgetContext")
        {
            Database.CommandTimeout = 0;
        }

        public DbSet<Budget2019> Budget2019s { get; set; }

    }
}
