using System.Data.Common;
using System.Data.Entity;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Models
{
    public abstract class RevelContextBase : DbContext, IRevelDBContextable
    {
        public RevelContextBase(string name)
            : base(name)
        {

        }

        public RevelContextBase(DbConnection dbConnection)
            : base(dbConnection, true)
        {

        }

        public virtual IDbSet<HouseAccount> HouseAccounts { get; set; }
        public virtual IDbSet<Employee> Employees { get; set; }
        public virtual IDbSet<HouseAccountPayment> HouseAccountPayments { get; set; }
        public virtual IDbSet<Address> Addresses { get; set; }
        public virtual IDbSet<OpeningHours> OpeningHours { get; set; }
        public virtual IDbSet<Establishment> Establishments { get; set; }
        public virtual IDbSet<Order> Orders { get; set; }
        public virtual IDbSet<OrderItem> OrderItems { get; set; }

        public virtual IDbSet<OrderAllInOne> OrdersAllInOne { get; set; }

        public virtual IDbSet<Product> Products { get; set; }
        public virtual IDbSet<ProductClass> ProductClasses { get; set; }
        public virtual IDbSet<ProductCategory> ProductCategories { get; set; }

        public virtual IDbSet<Payment> Payments { get; set; }

        public virtual IDbSet<Discount> Discounts { get; set; }

        public virtual IDbSet<Customer> Customers { get; set; }

        public virtual IDbSet<RewardsCardNew> RewardsCardNew { get; set; }

        public virtual IDbSet<GiftCard> GiftCards { get; set; }

        public virtual IDbSet<RewardsCardDailyPoints> RewardsCardDailyPoints { get; set; }

        public virtual IDbSet<RewardsPointsMultiplier> RewardsPointsMultiplier { get; set; }

        public virtual IDbSet<RewardCardPointsTransactionLog> RewardCardPointsTransactionLogs { get; set; }

        public virtual IDbSet<ScheduledTaskLog> ScheduledTaskLogs { get; set; }
        public virtual IDbSet<Brand> Brands { get; set; }
        public IDbSet<RewardCardLog> RewardCardLogs { get; set; }
        public IDbSet<SystemLog> SystemLogs { get; set; }

        public virtual IDbSet<LoyaltyCardType> LoyaltyCardTypes { get; set; }

        public virtual IDbSet<MenuType> MenuTypes { get; set; }

        public virtual IDbSet<Menu> Menus { get; set; }

        public virtual IDbSet<MenuFile> MenuFiles { get; set; }

        public virtual IDbSet<SystemError> SystemErrors { get; set; }


    }
}
