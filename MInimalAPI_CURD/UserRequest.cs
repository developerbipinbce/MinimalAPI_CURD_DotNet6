using System.ComponentModel.DataAnnotations;

namespace MInimalAPI_CURD
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
