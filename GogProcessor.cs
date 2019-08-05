using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class GogProcessor
    {
        public static async Task<GogGameModel> LoadGame(string gameName)
        {
            string urlSearch = "embed.gog.com/games/ajax/filtered?mediaType=game&search=" + gameName;

            using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlSearch))
            {
                if (response.IsSuccessStatusCode)
                {
                    var root = JsonConvert.DeserializeObject<SteamApplistResultModel>(await response.Content.ReadAsStringAsync());
                    var dict = root.applist.apps.ToDictionary(x => x.appid, x => x.name);
                    SteamGameResultModel gameData = dict[appID.ToString()];
                    return gameData.Data;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
