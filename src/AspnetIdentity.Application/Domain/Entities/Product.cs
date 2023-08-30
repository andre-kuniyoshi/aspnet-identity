using System.ComponentModel.DataAnnotations;

namespace AspnetIdentity.Application.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required, MaxLength(80, ErrorMessage = "Name cannot exceed 80 characters")]
        public string? Name { get; set; }

        public decimal Price { get; set; }
    }
}
