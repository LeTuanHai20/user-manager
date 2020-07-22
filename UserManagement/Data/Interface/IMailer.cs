using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Data.Interface
{
    public interface IMailer
    {
        public Task SendEmail(string content, string ToEmail, string subject, string Title);
    }
}
