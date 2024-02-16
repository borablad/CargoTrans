using CargoTrans.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CargoTrans.Services
{
    public class MockDataStore : IDataStore
    {
        public static string hosturl = "https://ktzh.shit-systems.dev/api";

        public MockDataStore()
        {
            
        }

        public async Task<List<string>> GetPoints()
        {
            List<string> result = new List<string>();
            try
            {
                string apiUrl = $"{hosturl}/point/?page=0&size=0";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        JObject responseObject = JObject.Parse(jsonContent);
                        JArray resultsArray = (JArray)responseObject["result"];

                        foreach (JToken rslt in resultsArray)
                        {
                            string title = rslt["title"].ToString();
                            result.Add(title);
                        }
                    }
                    else
                    {
                        // Обработка ошибок или иных сценариев
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
            }
            return result;
        }

    }
}