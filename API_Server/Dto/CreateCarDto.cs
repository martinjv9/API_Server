using System.ComponentModel.DataAnnotations;

namespace API_Server.Dto
{
    public class CreateCarDto
    {
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        public int CarCompnyId { get; set; }
    }
}
