using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SteamCompare
{
    public class GogProcessor
    {
        public static async Task<GogSearchModel> LoadGame(string search)
        {
            string urlSearch = "https://embed.gog.com/games/ajax/filtered?mediaType=game&search=";
            GogSearchResultModel root;
            Stack<GogSearchModel> searchStack;

            using (HttpResponseMessage response = await ApiHelper.apiClient.GetAsync(urlSearch + search))
            {
                if (response.IsSuccessStatusCode)
                {
                    root = JsonConvert.DeserializeObject<GogSearchResultModel>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            searchStack = new Stack<GogSearchModel>(root.products.FindAll(x => x.title.ToLower().Contains(search.ToLower())));

            while (searchStack.Count > 0)
            {
                GogSearchModel gameData = searchStack.Pop();
                if (gameData.type == 1)
                {
                    if (gameData.price.finalAmount == "0.00")
                        gameData.price.finalAmount = "FREE";
                    else
                        gameData.price.finalAmount = "$" + gameData.price.finalAmount;
                    return gameData;
                }
                
            }
            return null;
        }
    }
}
