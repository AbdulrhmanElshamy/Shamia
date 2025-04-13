using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamia.DataAccessLayer.Entities
{
    public class User : IdentityUser
    {
        public string City { get; set; }
        public string Role { get; set; } 
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public ICollection<Order> Orders { get; set; }
    }
}
