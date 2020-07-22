using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class User:IdentityUser
    {
        public bool Approved { get; set; }
        public InfoUser infoUser { get; set; }
    }
}
