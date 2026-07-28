using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes;

namespace Revel._808nd.com.ReportingModel
{
    public class ProductWatch
    {
        [Key]
        public int Id { get; set; }
        public int Revel_Product_Id { get; set; }                   
    }
}
