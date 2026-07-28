using data.pipeline.fourth.com.Interfaces.Public;
using System;
using System.ComponentModel.DataAnnotations;

namespace data.pipeline.fourth.com.Models
{
    public class PipelineRunRecord : IAmTimestampable
    {
        [Key]
        public int Id { get; set; }

        public int? BrandId { get; set; }
        [StringLength(256)]
        public string BrandName { get; set; }

        public int? StoreId { get; set; }
        [StringLength(256)]
        public string StoreName { get; set; }

        public int? StoreIntegrationId { get; set; }

        [StringLength(128)]
        public string SquareLocationId { get; set; }

        [StringLength(128)]
        public string FourthUnitId { get; set; }

        [StringLength(128)]
        public string FourthLocationCode { get; set; }

        [StringLength(64)]
        public string SourceSystem { get; set; }

        [StringLength(64)]
        public string TargetSystem { get; set; }

        [StringLength(64)]
        public string DataType { get; set; }

        [StringLength(64)]
        public string Status { get; set; }

        public DateTime PeriodStartUtc { get; set; }
        public DateTime PeriodEndUtc { get; set; }
        public DateTime TransactionDate { get; set; }

        [StringLength(512)]
        public string OutputFileName { get; set; }

        [StringLength(1024)]
        public string OutputFullPath { get; set; }

        public int RowCount { get; set; }

        [StringLength(32)]
        public string PayloadFormat { get; set; }

        public string Payload { get; set; }

        public int? FourthStatusCode { get; set; }
        public string FourthResponseBody { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime WhenCreatedUTC { get; set; }
        public DateTime WhenUpdatedUTC { get; set; }
    }
}
