using System.ComponentModel.DataAnnotations;

namespace BlazorEcommerce.Shared
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; } = String.Empty;

        [Required]
        public string Password { get; set; } = String.Empty;


    }
}
