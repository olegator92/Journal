﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Journal3.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Запомнить в браузере?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Электронный адрес")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "ID")]
        public string UserId { get; set; }

        [Display(Name = "Роль")]
        public string UserRoles { get; set; }

        [Required]
        [Display(Name = "График работы")]
        public int WorkSchedules { get; set; }

        [Display(Name = "Ключ")]
        public string Key { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен содержать как минимум {2} знаков.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPassViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен содержать как минимумt {2} знаков.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }
    }
}
