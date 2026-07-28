using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Models.Discounts
{
    public class PerSiteDiscount
    {
        public int EstablishmentId { get; set; } = 0;
        public decimal DiscountSalesPercentage { get; set; } = 0;

        public static IList<PerSiteDiscount> GetSiteDiscountPercentages()
        {
            return new List<PerSiteDiscount>
        {
            new PerSiteDiscount {
                EstablishmentId = 1 ,
                DiscountSalesPercentage = 0.04M
            },
               new PerSiteDiscount {
                EstablishmentId = 3 ,
                DiscountSalesPercentage = 0.065M
            },
               new PerSiteDiscount {
                EstablishmentId = 4 ,
                DiscountSalesPercentage = 0.04M
            },
               new PerSiteDiscount {
                EstablishmentId = 5 ,
                DiscountSalesPercentage = 0.1M
            },
                  new PerSiteDiscount {
                EstablishmentId = 6,
                DiscountSalesPercentage = 0.105M
            },
                     new PerSiteDiscount {
                EstablishmentId = 7,
                DiscountSalesPercentage = 0.022M
            },

                     new PerSiteDiscount {
                EstablishmentId = 8,
                DiscountSalesPercentage = 0.06M
            },
                new PerSiteDiscount {
                EstablishmentId = 9,
                DiscountSalesPercentage = 0.15M
            },
                new PerSiteDiscount {
                EstablishmentId = 10,
                DiscountSalesPercentage = 0.04M
            },
                new PerSiteDiscount {
                EstablishmentId = 11,
                DiscountSalesPercentage = 0.22M
            },
                new PerSiteDiscount {
                EstablishmentId = 13,
                DiscountSalesPercentage = 0.04M
            },
                new PerSiteDiscount {
                EstablishmentId = 14,
                DiscountSalesPercentage = 0.04M
            },
        };
        }
    }



}
