using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ApplicationUserViewModel
    {
        public string Id { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập họ tên.")]
        public string FullName { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập tên tài khoản.")]
        public string UserName { set; get; }

        public string Password { set; get; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime BirthDay { set; get; }

        [Required(ErrorMessage = "Bạn cần nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng.")]
        public string Email { set; get; }

        public string Address { set; get; }

        public string PhoneNumber { set; get; }

        public IEnumerable<ApplicationGroupViewModel> Groups { set; get; }
    }
}