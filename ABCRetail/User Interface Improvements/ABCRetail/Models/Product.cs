using ABCRetail.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string? ProductName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double Price { get; set; }

        [Required]
        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<OrderDetail> OrderDetail { get; set; }

        public List<CartDetail> CartDetail { get; set; }

        public Stock Stock { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string? Description { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }

        [NotMapped]
        public int Quantity { get; set; }
    }
}
