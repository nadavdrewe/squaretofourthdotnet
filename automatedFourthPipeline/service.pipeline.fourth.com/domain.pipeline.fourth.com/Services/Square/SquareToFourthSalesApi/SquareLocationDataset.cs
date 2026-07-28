using Square;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Services.Square
{
    /// <summary>
    /// Holds 'per location' data for Fourth sales transactions
    /// </summary>
    public class SquareLocationSalesDataset
    {
        public Location Location { get; set; }
        public IEnumerable<Order> orders { get; set; }
        public IEnumerable<Payment> paymentsForOrders { get; set; }
        public IEnumerable<PaymentRefund> refundsForOrders { get; set; }
    }

    public class SquareBrandSalesDataset
    {
        public IEnumerable<CatalogObject> entireCatalog { get; set; }
        public IEnumerable<CatalogObjectItemVariation> allProductVariations { get; set; }
        public IEnumerable<CatalogObjectItem> allItems { get; set; }
        public IEnumerable<CatalogObjectCategory> allCategories { get; set; }
        public IEnumerable<CatalogModifierList> allModifiers { get; set; }
        public IEnumerable<TeamMember> allEmployees { get; set; }
    }
}
