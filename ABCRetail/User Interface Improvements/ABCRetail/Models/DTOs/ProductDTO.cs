using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail.Models.DTOs;
public class ProductDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? ProductName { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    public double Price { get; set; }

    public string? Image { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public IFormFile? ImageFile { get; set; }

    public IEnumerable<SelectListItem>? CategoryList { get; set; }
}
