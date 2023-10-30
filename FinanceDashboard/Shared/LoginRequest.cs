using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared
{
    public class LoginRequest
    {
        //[Required]
        //[EmailAddress]
        public string? UserLogin { get; set; }
        //[Required]
        public string? Password { get; set; }
    }
}
