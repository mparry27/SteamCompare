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
            SteamApplistResultModel root = null;
            bool updated = false;
            try
            {
                root = JsonConvert.DeserializeObject<SteamApplistResultModel>(File.ReadAllText(@"..\..\SteamAppList.json"));
            }
            catch (FileNotFoundException)
            {
                UpdateAppList();
            }
            Stack<SteamApp> appList = new Stack<SteamApp>(root.applist.apps.FindAll(x => x.name.ToLower().Contains(gameName.ToLower())));
            SteamGameResultModel game = await SearchAppList(appList);

            if (game != null)
                return game.Data;
            else if (!updated)
            {
                UpdateAppList();
                appList = new Stack<SteamApp>(root.applist.apps.FindAll(x => x.name.ToLower().Contains(gameName.ToLower())));
                game = await SearchAppList(appList);
                if (game != null)
                    return game.Data;
                else
                    return null;
            }
            else
            {
                return null;
            }

            async void UpdateAppList()
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
                        updated = true;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }

            async Task<SteamGameResultModel> SearchAppList(Stack<SteamApp> appStack)
            {
                while (appStack.Count > 0)
                {
                    using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlGame + appStack.Peek().appid.ToString()))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, SteamGameResultModel>>(await response.Content.ReadAsStringAsync());
                            if (dict[appStack.Peek().appid.ToString()].Data != null && dict[appStack.Peek().appid.ToString()].Data.type == "game")
                            {
                                SteamGameResultModel gameData = dict[appStack.Peek().appid.ToString()];
                                if (gameData.Data.price_overview == null)
                                {
                                    gameData.Data.price_overview = new SteamPriceModel();
                                    gameData.Data.price_overview.final_formatted = "FREE";
                                }
                                return gameData;
                            }
                            appStack.Pop();
                        }
                        else
                            appStack.Pop();
                    }
                }
                return null;
            }
        }
    }
}
