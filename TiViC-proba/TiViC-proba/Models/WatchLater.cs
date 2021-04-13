using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiViC_proba.Models
{
    public class WatchLater
    {
        public string userId { get; set; }
        public string kanalId { get; set; }
        public string datum { get; set; }
        public string vreme { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }
        public string tip { get; set; }
    }
}
