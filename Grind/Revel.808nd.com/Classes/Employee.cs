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
    public class Employee : IRevelCreateable, IRevelAddressable
    {
        public bool active { get; set; }
        public string brand { get; set; }
        public string created_by { get; set; }
        public DateTime? created_date { get; set; }
        public string email { get; set; }
        public string employee_card { get; set; }
        //public string employee_end { get; set; }
        //public string employee_lastlogin { get; set; }
        public DateTime? employee_start { get; set; }
        public bool exempt { get; set; }
        public string external_id { get; set; }
        public int failed_login_attempts { get; set; }
        public string first_name { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public string internal_empl_id { get; set; }
        public string last_name { get; set; }
        public object last_timesheet_entry { get; set; }
        public bool locked_account { get; set; }
        //public string locked_account_timestamp { get; set; }
        public string mileage_reimbursement { get; set; }
        public string password_history { get; set; }
        public List<object> permissions { get; set; }
        public string phone_number { get; set; }
        //public string picture { get; set; }
        public string pin { get; set; }
        public string resource_uri { get; set; }
        public List<object> roles { get; set; }
        public string updated_by { get; set; }
        public DateTime? updated_date { get; set; }
        public string user { get; set; }
        [JsonIgnore]
        public string FourthEmpNo { get; set; }
        [JsonIgnore]
        public string FourthLocation { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string theAddress { get => "/resources/Employee?format=json&limit=0"; set => throw new NotImplementedException(); }

        public int Create(dynamic Type)
        {
            throw new NotImplementedException();
        }
    }
}

