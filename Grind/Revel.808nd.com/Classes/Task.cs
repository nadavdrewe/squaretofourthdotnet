using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class FourthTask
    {
        public enum FourthSuccessCode { Fail=0, Success=1 }

        [Key]
        public int FourthTaskId {get;set;}
        public string TaskExecutionTime { get;set; }
        public string TaskName { get; set; }
        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }
        public string TotalValue { get; set; }
        public string Message { get; set; }
        public FourthSuccessCode Code { get; set; }
    }
}
