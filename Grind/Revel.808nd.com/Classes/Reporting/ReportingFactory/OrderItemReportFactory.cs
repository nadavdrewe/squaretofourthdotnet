using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Revel._808nd.com.ReportingModel;
using Revel._808nd.com.ReportingModel.ChartModels;

namespace Revel._808nd.com.Classes.Reporting.ReportingFactory
{
    public class OrderItemReportFactory : ICreateProductOrderItemReport

    {
        public List<ProductOrderItemSummary> CreateProductOrderItemSummaryReport(ReportContext context, GrindContext db, out List<OrderItem> orderItemsOutPut, string UserURI = "")
        {
            db = (GrindContext)db;

            var whichGrind = context.IdOfStore;
            Console.WriteLine("Starting Grind" + whichGrind);

            var revelTImeStart = context.StartDate;
            var revelTImeEnd = context.EndDate;

            var overallqueryStartDate = revelTImeStart;
            var overallqueryEndDate = revelTImeEnd;
            var cycleDays = context.NoOfDaysInEachReportingPeriod;

            var catContainers = new List<CategoryProductOrderItemSummaryQueryContainer>();
            var currentCycleStartDate = overallqueryStartDate;
            var currentCycleEndDate = currentCycleStartDate.AddDays(cycleDays);
            orderItemsOutPut = new List<OrderItem>();


            /*Create the containers*/
            while (currentCycleStartDate < overallqueryEndDate)
            {


                catContainers.Add(new CategoryProductOrderItemSummaryQueryContainer
                {
                    StartDate = currentCycleStartDate,
                    EndDate = currentCycleEndDate,
                    CategoryProductOrderItemSummaries = new List<CategoryProductOrderItemSummary>(),
                    PeriodType = cycleDays + " days",
                });

                currentCycleStartDate = currentCycleStartDate.AddDays(cycleDays);
                currentCycleEndDate = currentCycleStartDate.AddDays(cycleDays);


            }





            var estId = whichGrind;
            var est = db.Establishments.First(x => x.establishment_id == estId);
            var prodCats = db.ProductCategories.Where(x => x.establishment_id == est.establishment_id).ToList();

            var allProds = db.Products.ToList();
            var productsForEst = new List<Product>();



            foreach (var p in allProds)
            {
                foreach (var c in prodCats)
                {

                    if (c.productcategory_id == p.categoryID)
                    {
                        productsForEst.Add(p);
                    }

                }
            }


            //ALL setup completed
            /*Populate the containers*/
            foreach (var container in catContainers)
            {

                var stDt = container.StartDate;
                var endDt = container.EndDate;
                var productOrderItemSummary = new ProductOrderItemSummary();

                try
                {


                    var items = GetOrderItemsForReporting(db, stDt, endDt);
                    orderItemsOutPut = items; //OUTPUT PARAM


                    if (!String.IsNullOrEmpty(UserURI))
                    {
                        items = items.Where(x => x.created_by == UserURI).ToList();
                    }


                    //get orders that are split bill
                    var OrdersSplitBills = GetOrdersThatAreSplitBills(db, stDt, endDt);




                    //EXCLUDE THE SPLIT BILL ITEMS
                    var itemToExlude = GetSplitBillOrderItems(OrdersSplitBills, items);


                    if (itemToExlude.Count > 0)
                    {
                        ExcludeOrderItems(itemToExlude, items);
                    }


                    //TESTING A PRODUCT
                    //	items.Where(item => item.Sku == "100000007433").ToList().Dump();


                    var prods = db.Products.ToList();
                    var cats = db.ProductCategories.ToList();

                    var itemsForEstablishmentProducts = new List<OrderItem>();

                    foreach (var item in items)
                    {
                        foreach (var prod in productsForEst)
                        {
                            if (item.product_id == prod.product_id)
                            {

                                itemsForEstablishmentProducts.Add(item);
                            }
                        }
                    }


                    var groupedItemsByProductSKU = itemsForEstablishmentProducts.GroupBy(x => x.product_id).ToList();




                    productOrderItemSummary.StoreType = "Establishment";
                    productOrderItemSummary.StoreIdentifier = est.name;


                    var ProductOrderItemSummaries = new List<ProductOrderItemSummary>();


                    /*Create if there are any, else add blank record so it appears on CSV*/
                    if (groupedItemsByProductSKU.Any())
                    {
                        foreach (var x in groupedItemsByProductSKU)
                        {
                            var prodId = x.First().product_id;
                            var prod = prods.Where(p => p.product_id == prodId).First();
                            var prodCat = cats.First(p => p.productcategory_id == prod.categoryID);


                            //vars
                            var totalPureSales = 0.00M;
                            var disc = 0.00M;

                            //GET TOTALS
                            try
                            {
                                totalPureSales = x
                                    .Where(i => i.voided_date == null)
                                    .Sum(i => i.pure_sales);
                            }
                            catch (Exception ex)
                            {

                            }


                            ProductOrderItemSummaries.Add(new ProductOrderItemSummary
                            {
                                StartDate = stDt,
                                EndDate = endDt,
                                //TotalDiscountTax = x.Where(i => i.discount_taxed == true).Sum(i => i.tax_amount),
                                TotalDiscount = x.Sum(i => i.discount_amount),
                                TotalPureSales = x
                                    .Where(i => i.voided_date == null)
                                    .Sum(i => i.pure_sales),
                                TotalTax = x
                                    .Where(i => i.voided_date == null)
                                    .Sum(i => i.tax_amount),
                                DifferentProducts = x.Count(),
                                ProductId = x.First().product_id,
                                ProductIdentifier = prod.name,
                                CategoryName = prodCat.name,
                                SKU = x.First().sku,
                                ProductCategoryId = prodCat.productcategory_id,
                                StoreIdentifier = productOrderItemSummary.StoreIdentifier,
                                StoreType = productOrderItemSummary.StoreType,
                                TotalQuantity =
                                    x.Where(i => i.voided_date == null)
                                        .Sum(i => i.quantity),
                                TotalVoided = x
                                    .Where(i => i.ervc_type == "5" || i.ervc_type == "6")
                                    .Sum(i => i.quantity),
                                TotalComps = x
                                    .Where(i => i.ervc_type == "7" || i.ervc_type == "8" || i.ervc_type == "9")
                                    .Sum(i => i.quantity)
                            });

                        }


                    }
                    /*There were no items, so add a blank row*/
                    else
                    {

                        ProductOrderItemSummaries.Add(new ProductOrderItemSummary
                        {
                            StartDate = stDt,
                            EndDate = endDt,
                            //TotalDiscountTax = x.Where(i => i.discount_taxed == true).Sum(i => i.tax_amount),
                            TotalDiscount = 0,
                            TotalPureSales = 0,
                            TotalTax = 0,
                            DifferentProducts = 0,
                            ProductId = 0,
                            ProductIdentifier = "Empty Records",
                            CategoryName = "No Category - No Products Solds",
                            SKU = "000000",
                            ProductCategoryId = 0,
                            StoreIdentifier = productOrderItemSummary.StoreIdentifier,
                            StoreType = productOrderItemSummary.StoreType,
                            TotalQuantity =
                            0,
                            TotalVoided = 0,
                            TotalComps = 0
                        });
                    }




                    var categoryProductSummaries = new List<CategoryProductOrderItemSummary>();


                    foreach (var p in ProductOrderItemSummaries)
                    {
                        /*Check there are items to generate off*/
                        if (p.TotalPureSales > 0)
                        {
                            var thisProduct = productsForEst.Where(x => x.product_id == p.ProductId).First();
                            var thisProductsCategory =
                                prodCats.First(x => x.productcategory_id == thisProduct.categoryID);

                            //create a new summary if none exists
                            if (
                                categoryProductSummaries.FirstOrDefault(
                                    x => x.ProductCategoryId == thisProductsCategory.productcategory_id) == null)
                            {
                                categoryProductSummaries.Add(new CategoryProductOrderItemSummary
                                {
                                    CategoryName = thisProductsCategory.name,
                                    ProductCategoryId = thisProductsCategory.productcategory_id,
                                    productOrderSummaries = new List<ProductOrderItemSummary>()
                                });
                            }

                            var catSummaryWeWant =
                                categoryProductSummaries.First(
                                    x => x.ProductCategoryId == thisProductsCategory.productcategory_id);
                            catSummaryWeWant.productOrderSummaries.Add(p);
                        }
                        /*It's empty again, generate some defaults*/
                        else
                        {
                            categoryProductSummaries.Add(new CategoryProductOrderItemSummary
                            {
                                CategoryName = "None",
                                ProductCategoryId = 0,
                                productOrderSummaries = new List<ProductOrderItemSummary>()
                            });

                            var catSummaryWeWant =
                               categoryProductSummaries.First(
                                   x => x.ProductCategoryId == 0);
                            catSummaryWeWant.productOrderSummaries.Add(p);
                        }

                    }

                    foreach (var catSummary in categoryProductSummaries)
                    {

                        if (catSummary.productOrderSummaries.Count() > 0)
                            catSummary.SetAttributes();
                    }

                    //set up final container for each category
                    var anythingTHere =
                        categoryProductSummaries.Where(
                            x => x.StartDate >= container.StartDate && x.EndDate <= container.EndDate).ToList();

                    if (anythingTHere.Count() > 0)
                    {
                        container.CategoryProductOrderItemSummaries.AddRange(
                            categoryProductSummaries.Where(
                                x => x.StartDate >= container.StartDate && x.EndDate <= container.EndDate).ToList());
                        container.SetAttributes();
                    }
                    else
                    {
                        container.SetNullAttributes();
                    }

                }
                catch (Exception ex)
                {
                    //one of the periods has failed - log it? 
                    db.SystemLogs.Add(new SystemLog
                    {
                        WhenCreated = DateTime.Now,
                        Type = "REPORTING_WEEKLY_ACCOUNTING",
                        WhoTriggered = "OrderItemReportFactory - CreateProductOrderItemSummaryReport()",
                        Note = "Grind: " + est.name + " Dates" + stDt + " - " + endDt.ToString() + "Exception: " + ex.StackTrace
                    });

                    db.SaveChanges();
                    throw;
                }


            }



            var bigCompiledListOfProductOrderItemsSummaries = new List<ProductOrderItemSummary>();

            for (int i = 0; i < catContainers.Count; i++)
            {
                var currentPeriod = i + 1;
                catContainers[i].Period = currentPeriod;

                var catCont = catContainers[i];
                var summary = catCont.CategoryProductOrderItemSummaries.ToList();
                summary.ForEach(x => x.productOrderSummaries.ForEach(y => y.PeriodNumber = currentPeriod));
                summary.ForEach(x => x.productOrderSummaries.ForEach(y => y.Period = "Week"));
                summary.ForEach(x => x.productOrderSummaries.ForEach(y => bigCompiledListOfProductOrderItemsSummaries.Add(y)));

            }

            return bigCompiledListOfProductOrderItemsSummaries.OrderBy(x => x.Period).ToList();
        }

        public static void ExcludeOrderItems(IList<OrderItem> itemToExlude, IList<OrderItem> items)
        {
            HashSet<int> itemsIds = new HashSet<int>(itemToExlude.Select(x => x.DBKEY_orderitem_id));

            foreach (var i in itemToExlude)
            {
                var item = items.Where(x => x.orderitem_id == i.orderitem_id).First();
                items.Remove(item);
            }
        }

        public static List<OrderItem> GetSplitBillOrderItems(IList<Order> OrdersSplitBills, IList<OrderItem> items)
        {
            List<OrderItem> itemToExlude = new List<OrderItem>();
            foreach (var order in OrdersSplitBills)
            {
                var cuerrentItemsToExlude = items.Where(x => x.parent_order_id == order.order_id).ToList();

                if (cuerrentItemsToExlude.Count > 0)
                {
                    foreach (var item in cuerrentItemsToExlude)
                    {
                        if (itemToExlude.FirstOrDefault(x => x.orderitem_id == item.orderitem_id) == null)
                        {
                            itemToExlude.Add(item);
                        }
                        else
                        {
                            var houstonWeHaveAProblem = "";
                        }
                    }
                }
            }
            return itemToExlude;
        }

        public static List<Order> GetOrdersThatAreSplitBills(GrindContext db, DateTime stDt, DateTime endDt)
        {
            var OrdersSplitBills = db.Orders
                .Where(x => x.created_date >= stDt)
                .Where(x => x.created_date <= endDt)
                .Where(x => x.bill_parent != null).ToList();
            return OrdersSplitBills;
        }

        public static List<OrderItem> GetOrderItemsForReporting(GrindContext db, DateTime stDt, DateTime endDt)
        {
            var items = db.OrderItems.AsNoTracking()
                .Where(x => x.created_date >= stDt)
                .Where(x => x.created_date <= endDt)
                .Where(x => x.deleted == false)
                //  .Where(x => x.voided_date == null)
                .ToList();
            return items;
        }


        public static IList<OrderItem> GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(GrindContext db, IList<ProductCategory> prodCats, DateTime startDate, DateTime endDate, out IList<OrderItem> voidsAndComps, out IList<Product> productsForEstablishment)
        {
            voidsAndComps = new List<OrderItem>();
            productsForEstablishment = new List<Product>();
            var items = OrderItemReportFactory.GetOrderItemsForReporting(db, startDate, endDate);


            ////////////////
            /// SPLIT BILLS
            ////////////////
            //get orders that are split bill
            var OrdersSplitBills = OrderItemReportFactory.GetOrdersThatAreSplitBills(db, startDate, endDate);


            //EXCLUDE THE SPLIT BILL ITEMS
            var itemToExlude = OrderItemReportFactory.GetSplitBillOrderItems(OrdersSplitBills, items);

            if (itemToExlude.Count > 0)
            {
                OrderItemReportFactory.ExcludeOrderItems(itemToExlude, items);
            }
            //SPLIT BILLS NOW EXCLUDED

            List<Product> productsForEst;
            var allItemsForEstablishmentProducts = GetItemsAndProductsForEstablishmentBasedOnProductCategories(db, prodCats, items, out productsForEst);

            var anyvoidsetc = allItemsForEstablishmentProducts
                .Where(i => i.ervc_type == "7" || i.ervc_type == "8" || i.ervc_type == "9"
                || i.ervc_type == "5"
                || i.ervc_type == "6").ToList();

            foreach (var voidOrWhatever in anyvoidsetc)
            {
                allItemsForEstablishmentProducts.Remove(voidOrWhatever);
            }

            productsForEstablishment = productsForEst;
            voidsAndComps = anyvoidsetc;
            return allItemsForEstablishmentProducts;

        }

        public static List<OrderItem> GetItemsAndProductsForEstablishmentBasedOnProductCategories(GrindContext db, IList<ProductCategory> prodCats, IList<OrderItem> items, out List<Product> productsForEst)
        {
            var itemsForEstablishmentProducts = new List<OrderItem>();

            var allProds = db.Products.ToList();
            productsForEst = new List<Product>();

            foreach (var p in allProds)
            {
                foreach (var c in prodCats)
                {
                    if (c.productcategory_id == p.categoryID)
                    {
                        productsForEst.Add(p);
                    }
                }
            }


            foreach (var item in items)
            {
                foreach (var prod in productsForEst)
                {
                    if (item.product_id == prod.product_id)
                    {
                        itemsForEstablishmentProducts.Add(item);
                    }
                }
            }
            return itemsForEstablishmentProducts;
        }


        public static OrderItemTypeCategoryBreakdown GetOrderItemTypeCategoryBreakdowns(IList<OrderItem> allItemsForEstablishmentProducts, Establishment est, IList<Product> productsForEstablishment, IEnumerable<ProductClass> productClasses)
        {
            var prodWrapperService = new RevelProductAndCategoryWrapper();
            prodWrapperService.Products = productsForEstablishment.ToList();

            IList<Product> tempErrors = new List<Product>();
            IList<Product> errors = new List<Product>();

            var foodItemProds = prodWrapperService.GetProductsThatAreFoodByClass(productClasses, out tempErrors);
            var softDrinkProds = prodWrapperService.GetProductsThatAreSoftDrinksByClass(productClasses, out tempErrors);
            var hotDrinkProds = prodWrapperService.GetProductsThatAreHotDrinksByClass(productClasses, out tempErrors);
            var alcoholProds = prodWrapperService.GetProductsThatAreAlcoholByClass(productClasses, out tempErrors);

            var itemsFood = new List<OrderItem>();
            var itemsSoftDrink = new List<OrderItem>();
            var itemsHotDrink = new List<OrderItem>();
            var itemsAlcohol = new List<OrderItem>();
            var itemsOther = new List<OrderItem>();

            foreach (var item in allItemsForEstablishmentProducts)
            {
                if (foodItemProds.FirstOrDefault(x => x.product_id == item.product_id) != null)
                {
                    itemsFood.Add(item);
                }
                else if (softDrinkProds.FirstOrDefault(x => x.product_id == item.product_id) != null)
                {
                    itemsSoftDrink.Add(item);
                }
                else if (hotDrinkProds.FirstOrDefault(x => x.product_id == item.product_id) != null)
                {
                    itemsHotDrink.Add(item);
                }
                else if (alcoholProds.FirstOrDefault(x => x.product_id == item.product_id) != null)
                {
                    itemsAlcohol.Add(item);
                }
                else
                {
                    itemsOther.Add(item);
                }
            }
            return new OrderItemTypeCategoryBreakdown
            {
                FoodItems = itemsFood,
                OtherItems = itemsOther,
                SoftDrinkItems = itemsSoftDrink,
                AlcoholItems = itemsAlcohol,
                HotDrinkItems = itemsHotDrink
            };

        }


        public static async Task<IEnumerable<HourAndSpend>> GetHourlySalesForOrderItems(DateTime start, DateTime end, List<OrderItemReportingItem> ordersItems)
        {


            //get the range of days
            var datesToCycle = extension.railgunit.com.DateTimeExtensions.DateRange(start, end);

            var allHourAndSpends = new List<HourAndSpend>();
            foreach (var date in datesToCycle)
            {

                var nextDay = date.AddDays(1);

                var allItemsInDateRange = ordersItems
                    .Where(x => x.CreatedDate >= date)
                    .Where(x => x.CreatedDate <= nextDay)
                    .ToList();


                //create hours
                TimeSpan ts = nextDay - date;

                IEnumerable<int> hoursBetween = Enumerable.Range(0, (int)ts.TotalHours)
                    .Select(i => date.AddHours(i).Hour);


                var todaysHourAndSpends = new List<HourAndSpend>();

                var lastDateTimeUsed = new DateTime(date.Year, date.Month, date.Day, hoursBetween.First() - 1, 00, 00);
                foreach (var hour in hoursBetween)
                {
                    var dateTimeThisHour = lastDateTimeUsed.AddHours(1);
                    lastDateTimeUsed = dateTimeThisHour;

                    var dateTimeNextHour = dateTimeThisHour.AddHours(1);

                    var itemsForTheHours = allItemsInDateRange
                        .Where(x => x.CreatedDate >= dateTimeThisHour)
                        .Where(x => x.CreatedDate <= dateTimeNextHour).ToList();

                    var totalThisHour = itemsForTheHours.Sum(x => x.Amount);

                    var _3am = new DateTime(dateTimeThisHour.Year, dateTimeThisHour.Month, dateTimeThisHour.Day, 03, 00,
                        00);
                    var _6am = _3am.AddHours(3);

                    /*    var isThisHourBetween3and6Am = IsThisHourBetween3and6Am(DateTime dateTimeThisHour)*/

                    if (dateTimeThisHour <= end)
                    {
                        todaysHourAndSpends.Add(new HourAndSpend
                        {
                            Date = dateTimeThisHour,
                            Hour = hour.ToString(),
                            Value = totalThisHour
                        });
                    }


                }

                foreach (var hourAndSpend in todaysHourAndSpends)
                {
                    if (Convert.ToInt16(hourAndSpend.Hour) > 6)
                        allHourAndSpends.Add(hourAndSpend);
                }


                //get items for that day 

            }
            //get the range of hours

            //cycle through and sum the amount for each hours 

            //return
            return allHourAndSpends;
        }

        public static async Task<List<TopSeller>> GetTopSellers(List<OrderItem> ordersItems, IList<User> users, IEnumerable<ProductClass> productClasses, IList<Product> products = null, Establishment est = null)
        {


            var topSellerBuckets = new List<TopSeller>();

            foreach (var user in users)
            {
                var orderItemsForThisUser = ordersItems.Where(x => x.created_by == user.resource_uri).ToList();


                TopSeller topseller;
                if (orderItemsForThisUser.Count > 0)
                {
                    topseller = (new TopSeller
                    {
                        Name = user.first_name + " " + user.last_name,
                        NumberOfItems = orderItemsForThisUser.Sum(x => x.quantity).ToString(),
                        ValuePounds = orderItemsForThisUser.Sum(x => x.pure_sales)
                    });

                    if (products != null && est != null)
                    {
                        topseller.Breakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(
                            orderItemsForThisUser, est, products, productClasses);
                    }

                    topSellerBuckets.Add(topseller);
                }


            }
            //cycle orders, assign each one to the user who created it

            return topSellerBuckets.OrderByDescending(x => x.ValuePounds).ToList();

        }

        public static Dictionary<string, decimal> GetPoundValueFromTotalValue(Dictionary<string, decimal> percentages, decimal currentTotalSpend)
        {
            var moneyValues = new Dictionary<string, decimal>();
            foreach (var cat in percentages)
            {
                moneyValues.Add(cat.Key, cat.Value * currentTotalSpend);
            }
            return moneyValues;
        }


        public static Dictionary<string, decimal> GetCategoryPercentagesOfTotalOrderItems(IList<OrderItem> allItemsForEstablishmentProducts, Establishment est, IList<Product> productsForEst, IEnumerable<ProductClass> productClasses, out decimal averageOrderSizeInPounds)
        {
            if (allItemsForEstablishmentProducts.Count() > 0)
            {


                var totalSalesFigureForAllItems = allItemsForEstablishmentProducts.Sum(x => x.pure_sales);
                /*      var totalOrderIdsForThoseItems = allItemsForEstablishmentProducts.Select(x => x.parent_order_id).Distinct().ToList();
*/

                var totalOrderIdsForThoseItems = allItemsForEstablishmentProducts.Sum(x => x.quantity);

                averageOrderSizeInPounds = 0.00M;

                try
                {
                    averageOrderSizeInPounds = Convert.ToDecimal(totalSalesFigureForAllItems) /
                                               Convert.ToDecimal(totalOrderIdsForThoseItems);


                    OrderItemTypeCategoryBreakdown allItemsAsCategoryBreakdowns =
                        OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(allItemsForEstablishmentProducts, est,
                            productsForEst, productClasses);

                    var hotDrinksPercentage =
                        GetItemsAsPercentageSalesOfTotalOrders(
                            allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.pure_sales),
                            totalSalesFigureForAllItems, totalOrderIdsForThoseItems) * averageOrderSizeInPounds / 100.00M;
                    var foodPercentage =
                        GetItemsAsPercentageSalesOfTotalOrders(
                            allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems,
                            totalOrderIdsForThoseItems) * averageOrderSizeInPounds / 100.00M;
                    var softDrinksPercentage =
                        GetItemsAsPercentageSalesOfTotalOrders(
                            allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.pure_sales),
                            totalSalesFigureForAllItems, totalOrderIdsForThoseItems) * averageOrderSizeInPounds / 100.00M;
                    var alcoholPercentage =
                        GetItemsAsPercentageSalesOfTotalOrders(
                            allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.pure_sales),
                            totalSalesFigureForAllItems, totalOrderIdsForThoseItems) * averageOrderSizeInPounds / 100.00M;
                    var otherPercentage =
                        GetItemsAsPercentageSalesOfTotalOrders(
                            allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems,
                            totalOrderIdsForThoseItems) * averageOrderSizeInPounds / 100.00M;

                    //what are we returning?
                    var itemTypes = new List<string>
                    {
                        "Hot Drinks",
                        "Food",
                        "Soft Drinks",
                        "Alcohol",
                        "Others"
                    };

                    var dic = new Dictionary<string, decimal>();
                    dic.Add("Hot Drinks", hotDrinksPercentage);
                    dic.Add("Food", foodPercentage);
                    dic.Add("Soft Drinks", softDrinksPercentage);
                    dic.Add("Alcohol", alcoholPercentage);
                    dic.Add("Others", otherPercentage);


                    return dic;

                }
                catch (Exception ex)
                {
                    throw new DivideByZeroException(
                        "Couldn't get an average order size, there's probably zero records", ex);

                }

            }

            else
            {
                averageOrderSizeInPounds = 0.00M;
                return new Dictionary<string, decimal>();

            }


        }


        public static decimal GetItemsAsPercentageSalesOfTotalOrders(decimal specificItemSalesFigure, decimal totalItemSalesFigure, int numberOfOrders)
        {
            try
            {
                return (specificItemSalesFigure / totalItemSalesFigure) * 100.00M;
            }
            catch (Exception)
            {

                return 0.00M;
            }
        }

    }







}
