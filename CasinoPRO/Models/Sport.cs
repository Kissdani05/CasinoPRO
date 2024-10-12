using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CasinoPRO.OrganizerPage;

namespace CasinoPRO.Models
{
    public class Sport
    {
        public string Name { get; set; }
        public ObservableCollection<Bet> Bets { get; set; }

        public Sport(string name)
        {
            Name = name;
            Bets = new ObservableCollection<Bet>();
        }
    }
}
