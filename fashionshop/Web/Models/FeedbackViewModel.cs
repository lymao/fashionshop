using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class FeedbackViewModel
    {
        public int ID { set; get; }

        [MaxLength(250, ErrorMessage = "Tên không được quá 250 ký tự")]
        [Required(ErrorMessage = "Bạn chưa nhập họ tên")]
        public string Name { set; get; }

        [MaxLength(250, ErrorMessage = "Email không được quá 250 ký tự")]
        [Required(ErrorMessage = "Bạn chưa nhập email")]
        public string Email { set; get; }

        [MaxLength(500, ErrorMessage = "Tin nhắn không được quá 500 ký tự")]
        [Required(ErrorMessage = "Nhập số điện thoại của bạn để tiện liên lạc")]
        public string Message { set; get; }

        [MaxLength(50,ErrorMessage ="Số điện thoại chỉ tối đa 50 ký tự")]
        public string Phone { set; get; }

        public DateTime CreatedDate { set; get; }

        public bool Status { set; get; }

        public ContactDetailViewModel ContactDetail { set; get; }
    }
}