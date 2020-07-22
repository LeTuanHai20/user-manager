using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class EditRoleViewModel
    {
        public string ID { get; set; }
        [Require]
        public string  Name { get; set; }
    }
}
