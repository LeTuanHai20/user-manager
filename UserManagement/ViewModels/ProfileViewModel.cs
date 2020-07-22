using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class ProfileViewModel
    {
        public int infoId { get; set; }
        [Required]
        [MaxLength(40)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool Enable { get; set; }

    }
}
