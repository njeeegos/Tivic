using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiViC_proba.Models
{
    public class Programi_po_KanaluViewModel
    {
        public string KanalId { get; set; }
        public string Datum { get; set; }
        public IEnumerable<Kanal> Kanali { get; set; }
        public IEnumerable<string> Datumi { get; set; }
        public IEnumerable<Programme> Programi { get; set; }
        //public IEnumerable<Programme> Ponedeljak { get; set; }
        //public IEnumerable<Programme> Utorak { get; set; }
        //public IEnumerable<Programme> Sreda { get; set; }
        //public IEnumerable<Programme> Cetvrtak { get; set; }
        //public IEnumerable<Programme> Petak { get; set; }
        //public IEnumerable<Programme> Subota { get; set; }
        //public IEnumerable<Programme> Nedelja { get; set; }
    }
}
