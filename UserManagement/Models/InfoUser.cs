using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;

namespace UserManagement.Models
{
    public class InfoUser
    {
        [Key]
        public int InfoId { get; set; }
        public string FullName { get; set; }
        public bool Enable { get; set; }
        public string UserId { get; set; }
        public User IdentityUser { get; set; }
        
    }
}
