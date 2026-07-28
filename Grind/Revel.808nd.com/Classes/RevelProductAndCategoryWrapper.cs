using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.ServiceImplemenations;

namespace Revel._808nd.com.Classes
{
    public class RevelProductAndCategoryWrapper
    {
        public List<Establishment> Establishments { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        //dictionary for each comparison of categories and ID
        public Dictionary<int, string> ProductCategoriesComparisonDictionary { get; set; }


        public RevelProductAndCategoryWrapper()
        {

            this.Products = new List<Product>();
            this.ProductCategories = new List<ProductCategory>();
            this.ProductCategoriesComparisonDictionary = new Dictionary<int, string>();
        }

        public RevelProductAndCategoryWrapper(List<Establishment> establishments)
        {
            this.Establishments = establishments;
            this.Products = new List<Product>();
            this.ProductCategories = new List<ProductCategory>();
            this.ProductCategoriesComparisonDictionary = new Dictionary<int, string>();
        }


        /// <summary>
        /// Hot cold, food, drinks, booze etc
        /// </summary>
        public void Initialise(RevelContextBase db, IEnumerable<ProductClass> productClasses)
        {
            this.Products = db.Products.ToList();
            IList<Product> tempErrors = new List<Product>();
            IList<Product> errors = new List<Product>();

            foreach (var est in this.Establishments)
            {
                GetProductsThatAreHotDrinksByClass(productClasses, out tempErrors);
                GetProductsThatAreAlcoholByClass(productClasses, out tempErrors);
                GetProductsThatAreFoodByClass(productClasses, out tempErrors);
                GetProductsThatAreSoftDrinksByClass(productClasses, out tempErrors);
            }

        }
        /// <summary>
        /// Products and cats need to be 
        /// </summary>
        /// <returns></returns>
        public bool CreateProductCategoriesDictionary()
        {
            try
            {
                if (this.ProductCategories.Count > 0)
                {
                    foreach (var cat in ProductCategories)
                    {
                        ProductCategoriesComparisonDictionary.Add(cat.productcategory_id, cat.name);
                    }
                    return true;
                }
            }
            catch (Exception exception)
            {

                throw exception;
            }

            return false;
        }


        public Dictionary<string, int> GetProductCategoryBreakdown(List<OrderItem> orderItems)
        {
            Dictionary<string, int> productBreakdown = new Dictionary<string, int>();
            List<OrderItem> OrderItemErrorList = new List<OrderItem>();

            foreach (var oi in orderItems)
            {
                //test there is a ID...
                if (oi.product_id != null && oi.product_id != 0)
                {
                    try
                    {
                        Product product = this.Products.Where(c => c.product_id == oi.product_id).FirstOrDefault();
                        ProductCategory productCategory =
                            this.ProductCategories.Where(x => x.productcategory_id == product.categoryID).First();
                        /*              ProductCategory productCategoryParentCategory =
                                          this.ProductCategories.Where(x => x.productcategory_id == productCategory.parent_id).FirstOrDefault();
              */
                        string categoryName = productCategory.name;

                        //now cycle through a list of 


                        if (productBreakdown.ContainsKey(categoryName))
                        {
                            int value = productBreakdown[categoryName];
                            value += oi.quantity;
                            productBreakdown[categoryName] = value;
                        }
                        else
                        {
                            productBreakdown.Add(categoryName, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = oi;

                    }
                }
                else
                {
                    OrderItemErrorList.Add(oi);
                }
            }
            return productBreakdown;
        }



        public List<Product> GetProductsThatAreHotDrinksByClass(IEnumerable<ProductClass> allProductClasses, out IList<Product> errorProducts)
        {
            List<Product> productsThatAreCoffee = new List<Product>();
            errorProducts = new List<Product>();
            foreach (var product in this.Products)
            {
                try
                {
                    if (ProductClassService.GetParentRootClass(product, allProductClasses).name.ToLower() == "coffee/hot drinks")
                    {
                        productsThatAreCoffee.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    errorProducts.Add(product);
                }
            }

            return productsThatAreCoffee;
        }


        public List<Product> GetProductsThatAreFoodByClass(IEnumerable<ProductClass> allProductClasses, out IList<Product> errorProducts)
        {
            List<Product> productsThatAreCoffee = new List<Product>();
            errorProducts = new List<Product>();
            foreach (var product in this.Products)
            {
                try
                {
                    if (ProductClassService.GetParentRootClass(product, allProductClasses).name.ToLower() == "food")
                    {
                        productsThatAreCoffee.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    errorProducts.Add(product);
                }
            }

            return productsThatAreCoffee;
        }

        public List<Product> GetProductsThatAreSoftDrinksByClass(IEnumerable<ProductClass> allProductClasses, out IList<Product> errorProducts)
        {
            List<Product> productsThatAreCoffee = new List<Product>();
            errorProducts = new List<Product>();
            foreach (var product in this.Products)
            {
                try
                {
                    if (
                               ProductClassService.GetParentRootClass(product, allProductClasses).name.ToLower() == "soft drinks" ||
                               ProductClassService.GetParentRootClass(product, allProductClasses).name.ToLower() == "juice"
                               )
                    {
                        productsThatAreCoffee.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    errorProducts.Add(product);
                }
            }

            return productsThatAreCoffee;
        }

        public List<Product> GetProductsThatAreAlcoholByClass(IEnumerable<ProductClass> allProductClasses, out IList<Product> errorProducts)
        {
            List<Product> productsThatAreCoffee = new List<Product>();
            errorProducts = new List<Product>();
            foreach (var product in this.Products)
            {
                try
                {
                    if (ProductClassService.GetParentRootClass(product, allProductClasses).name.ToLower() == "bar")
                    {
                        productsThatAreCoffee.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    errorProducts.Add(product);
                }
            }

            return productsThatAreCoffee;
        }



        public bool isItemFood(OrderItem item, IEnumerable<Product> foodProducts, out IList<Product> errorProducts)
        {
            errorProducts = new List<Product>();
            foreach (var foodProduct in foodProducts)
            {
                try
                {
                    if (foodProduct.product_id == item.product_id)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    errorProducts.Add(foodProduct);
                    throw;
                }
            }

            return false;

        }



        public bool isItemAlcohol(OrderItem item, IEnumerable<Product> alcohol, out IList<Product> errorProducts)
        {

            try
            {
                errorProducts = new List<Product>();
                foreach (var prod in alcohol)
                {
                    if (prod.product_id == item.product_id)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }




        public bool isItemSoftDrink(OrderItem item, IEnumerable<Product> softDrinks, out IList<Product> errorProducts)
        {

            errorProducts = new List<Product>();
            foreach (var prod in softDrinks)
            {
                try
                {

                    if (prod.product_id == item.product_id)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    errorProducts.Add(prod);
                    throw;
                }
            }

            return false;

        }

        public bool isItemHotDrink(OrderItem item, IEnumerable<Product> hotDrinks, out IList<Product> errorProducts)
        {

            errorProducts = new List<Product>();
            foreach (var prod in hotDrinks)
            {
                try
                {
                    if (prod.product_id == item.product_id)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    errorProducts.Add(prod);
                    throw;
                }

            }

            return false;
        }

    }

}

