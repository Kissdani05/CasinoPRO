using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoPRO.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
}
