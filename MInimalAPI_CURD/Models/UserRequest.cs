using System.ComponentModel.DataAnnotations;

namespace MInimalAPI_CURD.Models
{
    public record UserDto(string UserName, string Password);
    public record UserRequst
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
