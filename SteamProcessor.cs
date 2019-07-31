using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class SteamProcessor
    {
        public static async Task<SteamGameModel> LoadGame(string gameName = "")
        {
            string appList = "http://api.steampowered.com/ISteamApps/GetAppList/v2/";
            int appID = 253230;
            string url = $"https://store.steampowered.com/api/appdetails/?appids={appID}";

            using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    SteamGameResultModel result = await response.Content.ReadAsAsync<SteamGameResultModel>();
                    return result.Data;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
