using System.ComponentModel.DataAnnotations;

namespace AspnetIdentity.Application.Domain.Entities
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required, MaxLength(80, ErrorMessage = "Name must be up to 80 characters")]
        public string? Name { get; set; }

        [EmailAddress]
        [Required, MaxLength(120)]
        public string? Email { get; set; }
        public int Age { get; set; }
    }
}
