using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class SteamProcessor
    {
        public static async Task<SteamGameModel> LoadGame(string gameName)
        {
            string urlApplist = "http://api.steampowered.com/ISteamApps/GetAppList/v2/";
            string urlGame = "https://store.steampowered.com/api/appdetails/?appids=";
            int appID = 0;

            using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlApplist))
            {
                if (response.IsSuccessStatusCode)
                {
                    var root = JsonConvert.DeserializeObject<SteamApplistResultModel>(await response.Content.ReadAsStringAsync());
                    var dict = root.applist.apps.ToDictionary(x => x.appid, x => x.name);
                    appID = dict.FirstOrDefault(x => x.Value == gameName).Key;
                    urlGame = urlGame + appID;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlGame))
            {
                if (response.IsSuccessStatusCode)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, SteamGameResultModel>>(await response.Content.ReadAsStringAsync());
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
