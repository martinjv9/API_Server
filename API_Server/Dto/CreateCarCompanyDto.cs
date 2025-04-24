using System.ComponentModel.DataAnnotations;

namespace API_Server.Dto
{
    public class CreateCarCompanyDto
    {

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CountryOrigin { get; set; } = string.Empty ;
    }
}
