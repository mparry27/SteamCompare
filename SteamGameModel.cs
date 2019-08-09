using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class SteamGameModel
    {
        public string name { get; set; }
        public int steam_appid { get; set; }
        public string type { get; set; }
        public bool is_free { get; set; }
        public SteamPriceModel price_overview { get; set; }
        public string header_image { get; set; }
    }
}
