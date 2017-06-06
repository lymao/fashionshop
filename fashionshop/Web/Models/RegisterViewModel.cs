using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Bạn cần nhập họ tên."), Display(Name = "Họ tên")]
        public string FullName { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập tên tài khoản."), Display(Name ="Tài khoản")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập mật khẩu."), Display(Name = "Mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải ít nhất là {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhật mật khẩu không đúng.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng.")]
        public string Email { set; get; }

        [Display(Name = "Địa chỉ")]
        public string Address { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập số điện thoại."), Display(Name = "Số điện thoại")]
        public string PhoneNumber { set; get; }
    }
}