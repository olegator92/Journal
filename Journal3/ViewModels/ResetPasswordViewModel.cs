using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Journal3.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ReturnToken { get; set; }
    }
}