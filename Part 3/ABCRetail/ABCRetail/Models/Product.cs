using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Product : ITableEntity
    {
        [Key]
        public int ProductId { get; set; }  // Ensure this property exists and is populated
        public string? ProductName { get; set; }  // Ensure this property exists and is populated
        public string? Description { get; set; }
        public string price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }

        // ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}