using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class TimeSheetEntry : IRevelAddressable, IRevelCreateable
    {
        public object break_length { get; set; }
        public object break_type { get; set; }
        public DateTime? clock_in { get; set; }
        public DateTime? clock_out { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string department_name { get; set; }
        public string employee { get; set; }
        public string establishment { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public object is_auto_clock_out { get; set; }
        public string resource_uri { get; set; }
        public string role_name { get; set; }
        public double role_wage { get; set; }
        public int stage { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string theAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public int Create(dynamic Type)
        {
            throw new NotImplementedException();
        }
    }
}
