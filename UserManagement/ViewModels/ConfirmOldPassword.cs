using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class ConfirmOldPassword
    {
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }
    }
}
