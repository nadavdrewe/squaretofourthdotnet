using System.Data.Entity;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.Interfaces
{
    public interface IRevelDBContextable
    {

        IDbSet<Address> Addresses { get; set; }
        IDbSet<OpeningHours> OpeningHours { get; set; }
        IDbSet<Establishment> Establishments { get; set; }
        IDbSet<Order> Orders { get; set; }
        IDbSet<OrderItem> OrderItems { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<ProductCategory> ProductCategories { get; set; }

        IDbSet<Payment> Payments { get; set; }

        IDbSet<Discount> Discounts { get; set; }

        IDbSet<Customer> Customers { get; set; }

        IDbSet<RewardsCardNew> RewardsCardNew { get; set; }

        IDbSet<GiftCard> GiftCards { get; set; }

        IDbSet<RewardsCardDailyPoints> RewardsCardDailyPoints { get; set; }

        IDbSet<RewardsPointsMultiplier> RewardsPointsMultiplier { get; set; }

        IDbSet<RewardCardPointsTransactionLog> RewardCardPointsTransactionLogs { get; set; }
        IDbSet<ScheduledTaskLog> ScheduledTaskLogs { get; set; }
        IDbSet<Brand> Brands { get; set; }
        IDbSet<RewardCardLog> RewardCardLogs { get; set; }
        IDbSet<SystemLog> SystemLogs { get; set; }




    }
}
