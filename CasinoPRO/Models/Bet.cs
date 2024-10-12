using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoPRO.Models
{
    public class Bet
    {
        public string Match { get; set; }
        public decimal HomeOdds { get; set; }
        public decimal AwayOdds { get; set; }
        public decimal DrawOdds { get; set; }
    }
}
