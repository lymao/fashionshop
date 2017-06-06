using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage ="Bạn cần nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage ="Bạn cần nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới phải ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu mới chưa đúng.")]
        public string ConfirmPassword { get; set; }
    }
}