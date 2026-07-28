using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting.Caternet;
using Revel._808nd.com.Classes.ServiceImplemenations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Grind
{
    public class ProductClassSummaryBucket
    {
        public IList<CaternetItemSummary> ProductItemSummaries { get; set; } = new List<CaternetItemSummary>();
        public string ClassName { get; set; }
        public bool Taxed { get; set; }


    }


    public class ProductClassSummaryBucketGenerator
    {
        IEnumerable<Product> _allProducts;
        IEnumerable<ProductClass> _allClasses;


        public ProductClassSummaryBucketGenerator(ProductClassService classService, IEnumerable<Product> allProducts, IEnumerable<ProductClass> allClasses)
        {
            _allProducts = allProducts;
            _allClasses = allClasses;
        }

        public IEnumerable<ProductClassSummaryBucket> GenerateBuckets(IEnumerable<CaternetItemSummary> summaries, IEnumerable<ProductClass> allClasses, IEnumerable<Product> allProducts)
        {
            List<ProductClassSummaryBucket> bucketsToReutrn = new List<ProductClassSummaryBucket>();
            List<CaternetItemSummary> errorSummariesNotIncluded = new List<CaternetItemSummary>();


            foreach (var item in summaries)
            {
                //try get parent category

                var prod = _allProducts.FirstOrDefault(x => x.product_id == item.ProductId);

                ProductClass parentClass = ProductClassService.GetParentRootClass(prod, _allClasses);
                var exist = bucketsToReutrn.FirstOrDefault(x => x.ClassName.ToLower().Trim() == parentClass.name.ToLower().Trim());
                if (exist != null)
                {
                    exist.ProductItemSummaries.Add(item);

                }
                else
                {
                    bucketsToReutrn.Add(new ProductClassSummaryBucket { ClassName = parentClass.name, ProductItemSummaries = new List<CaternetItemSummary> { item } });
                }

            }


            return bucketsToReutrn;

        }

    }


}
