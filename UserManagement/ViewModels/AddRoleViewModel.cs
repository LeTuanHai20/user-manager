using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class AddRoleViewModel
    {
       
        [Required]
        [MaxLength(20)]
        public string  RoleName { get; set; }
    }
}
