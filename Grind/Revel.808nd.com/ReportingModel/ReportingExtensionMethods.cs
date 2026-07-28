using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.ReportingModel
{
    public static class ReportingExtensionMethods
    {
        public static List<OrderItemReportingItem> FilterSplitBills(List<int> orderIdsToExclude, List<OrderItemReportingItem> items)
        {


            var orderItemsWithoutExcluded =
                items.Where(x => orderIdsToExclude.All(anId => anId != x.ParentOrderId)).ToList();


            return orderItemsWithoutExcluded;
        }


        public static List<OrderItemReportingItem> ToOrderItemReportingItems(this List<OrderItem> items) 
        {
            return items.Select(x => new OrderItemReportingItem
            {
                Id = x.orderitem_id,
                Amount = x.pure_sales,
                Quantity = x.quantity,
                ParentOrderId = x.parent_order_id,
                CreatedBy = x.created_by,
                ProductId = x.product_id,
                CreatedDate = x.created_date,
                ERVC_Type = x.ervc_type

            })
                .ToList();
        }



    }
}
