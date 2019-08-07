using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            SteamApplistResultModel root;
            Queue<SteamApp> appQueue;
            try
            {
                root = JsonConvert.DeserializeObject<SteamApplistResultModel>(File.ReadAllText(@"..\..\SteamAppList.json"));
            }
            catch (FileNotFoundException ex)
            {
                using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlApplist))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        root = JsonConvert.DeserializeObject<SteamApplistResultModel>(await response.Content.ReadAsStringAsync());
                        using (StreamWriter file = File.CreateText(@"..\..\SteamAppList.json"))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(file, root);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }

            appQueue = new Queue<SteamApp>(root.applist.apps.FindAll(x => x.name.Contains(gameName)));
            while (appQueue.Count != 0)
            {
                urlGame = urlGame + appQueue.Peek().appid;
                using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlGame))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, SteamGameResultModel>>(await response.Content.ReadAsStringAsync());
                        SteamGameResultModel gameData = dict[appQueue.Peek().appid.ToString()];
                        return gameData.Data;
                    }
                    else
                    {
                        appQueue.Dequeue();
                    }
                }
            }
            return null;
        }
    }
}
