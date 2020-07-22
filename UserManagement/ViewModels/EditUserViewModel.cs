using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class EditUserViewModel
    {
        public string ID { get; set; }
        [Required]
        [EmailAddress]
        
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Enable { get; set; }

    }
}
