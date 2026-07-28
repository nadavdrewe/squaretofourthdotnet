using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes

{
    public partial class ProductCategory : IRevelAddressable, IRevelCreateable
    {

       [Key]
        public int DBKEY_productcategory_id { get; set; }

                
        [JsonProperty("active")]
        public bool active { get; set; }
        /*
        [JsonProperty("brand")]
        public string brand { get; set; }

        [JsonProperty("color_code")]
        public int color_code { get; set; }

        [JsonProperty("created_by")]
        public string created_by { get; set; }

        [JsonProperty("created_date")]
        public string created_date { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }*/

        [JsonProperty("establishment")]
        public string establishment { get; set; }

        /*[JsonProperty("film")]
        public bool film { get; set; }

        [JsonProperty("film_rating")]
        public object film_rating { get; set; }
*/
 
        [JsonProperty("id")]
        public int productcategory_id { get; set; }
/*

        [JsonProperty("img_url")]
        public object img_url { get; set; }

        [JsonProperty("lock_enable")]
        public bool lock_enable { get; set; }

        [JsonProperty("lock_uuid")]
        public object lock_uuid { get; set; }

*/
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("parent")]
        public string parent { get; set; }

        [JsonIgnore]
        public int parent_id { get; set; }

        [JsonProperty("resource_uri")]
        public string resource_uri { get; set; }
/*
        [JsonProperty("sorting")]
        public int sorting { get; set; }

        [JsonProperty("subcategories")]
        public List<ProductCategory> subcategories { get; set; }

        [JsonProperty("updated_by")]
        public string updated_by { get; set; }

        [JsonProperty("updated_date")]
        public string updated_date { get; set; }

*/
        //added by ND
        
        
/*
        public int brand_id { get;  set; }
                
        public int parent_id { get;  set; }
*/
                
/*
        public List<int> subcatagories_id { get;  set; }
*/
               
        public int establishment_id { get;  set; }
     

        public ProductCategory()
        {
            theAddress = theAddress = "/products/ProductCategory/?format=json&limit=0"; 
        }
               
        [NotMapped]
        public string theAddress { get; set; }

        public int Create(dynamic Type)
        {
            

            productcategory_id = (int)Type["id"];
            name = (string)Type["name"];
            //    parent = (string)Type["parent"];
            //  parent_id =
            //    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
            //         (string)Type["parent"]);

            establishment = (string)Type["establishment"];
            establishment_id =
                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                    (string)Type["establishment"]);
            parent = (string) Type["parent"];
            active = (bool)Type["active"];
            resource_uri = (string)Type["resource_uri"];
            // subTypeegories = new List<ProductTypeegory>(
            //);


            /*
                                        //nested list for subTypes
                                        foreach (var nestedType in Type["subTypeegories"].Children())
                                        {
                                            ProductTypeegory anotherTypeegory = new ProductTypeegory();

                                 //           anotherTypeegory.brand = (string)nestedType["brand"];
                                            anotherTypeegory.productTypeegory_id = (int)nestedType["id"];
                                            anotherTypeegory.name = (string)nestedType["name"];
                                  //          anotherTypeegory.parent = (string)nestedType["parent"];
                                  //          anotherTypeegory.parent_id =
                                   //             RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                     //               (string)nestedType["parent"]);

                                            anotherTypeegory.establishment = (string)nestedType["establishment"];
                                            anotherTypeegory.establishment_id =
                                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                                    (string)nestedType["establishment"]);

                                        /*    anotherTypeegory.subTypeegories = new List<ProductTypeegory>(
                                            );


                                            subTypeegories.Add(anotherTypeegory);
            #1#
                                        }*/
            return 0;

        }
    }


    

}