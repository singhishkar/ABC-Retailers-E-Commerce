using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail.Models
{
    public class User : ITableEntity
    {
        [NotMapped]
        public int CustId { get; set; }

        public string CustEmail { get; set; }

        public string CustPassword { get; set; }

        public string CustPasswordHash { get; set; }

        public string PartitionKey { get; set; } = "Customers";

        // Static partition key for all customers
        public string RowKey { get; set; }  // Unique identifier, such as email or a GUID
        public DateTimeOffset? Timestamp { get; set; }

        [NotMapped]
        public ETag ETag { get; set; }

    }
}