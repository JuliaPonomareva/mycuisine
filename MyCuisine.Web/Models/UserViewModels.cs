using System.ComponentModel.DataAnnotations;

namespace MyCuisine.Web.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public UpdateNameFormModel UpdateNameForm { get; set; }
        public UpdatePasswordFormModel UpdatePasswordForm { get; set; }
        public class UpdateNameFormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [StringLength(maximumLength: 50, ErrorMessage = "Максимальная длина 50 символов.")]
            public string Name { get; set; }
        }

        public class UpdatePasswordFormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [MinLength(8, ErrorMessage = "Минимальная длина 8 символов.")]
            public string Password { get; set; }
        }
    }
}
