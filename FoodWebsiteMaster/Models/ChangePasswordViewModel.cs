using System.ComponentModel.DataAnnotations;

namespace FoodWebsiteMaster.Models
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; }

        [Required]
        //[MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
