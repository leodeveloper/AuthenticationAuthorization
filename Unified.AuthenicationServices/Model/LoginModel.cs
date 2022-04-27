using System.ComponentModel.DataAnnotations;

namespace Unified.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class WindowsLoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }
    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
        public bool ispasswordreset { get; set; }
    }
}
