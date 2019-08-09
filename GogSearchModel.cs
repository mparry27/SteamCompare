using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class GogSearchModel
    {
        public GogPriceModel price { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public int id { get; set; }
    }
}
