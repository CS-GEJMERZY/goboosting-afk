using Core.Models;
using CounterStrikeSharp.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Managers;

public class WebManager(string apiKey)
{
    private readonly string _apiKey = apiKey;
    private readonly int _apiVersion = 200;

    public async Task<List<PlayerWebResponseData>?> GetPlayersAsync(List<PlayerWebInputData> PlayerInputData, string ServerIp)
    {
        var jsonData = PlayerInputData.Select(player => new
        {
            steam64 = player.Steam64,
            name = player.PlayerName,
            kill = player.Kills,
            dead = player.Deaths,
            assist = player.Assists,
            team = player.Team

        });

        string jsonString = JsonConvert.SerializeObject(jsonData);

        string query = $"https://goboosting.pl/api.php?gracze&ip={ServerIp}&api={_apiKey}&ver={_apiVersion}&json={jsonString}";

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Valve/CSS Client");

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                return string.IsNullOrEmpty(responseContent) ? null : ParseReponse(responseContent);
            }
            else
            {
                throw new Exception($"API request failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred during API GetPlayersAsync request", ex);
        }
    }

    static List<PlayerWebResponseData> ParseReponse(string jsonData)
    {
        JArray playerArray = JArray.Parse(jsonData);
        List<PlayerWebResponseData> output = [];
        foreach (var ent in playerArray)
        {
            var data = new PlayerWebResponseData
            {
                Steam64 = (string)ent["steam64"]!,
                CreditsEarned = (float)ent["zarobek"]!,
                DoTest = (int)ent["testuj"]!
            };

            output.Add(data);
        }

        return output;
    }

    public async Task SendPlayerUpdate(PlayerMenuFailType FailType, string Steam64, string ServerIp)
    {
        string query = $"https://goboosting.pl/api.php?afk&api={_apiKey}&ip={ServerIp}&steam64={Steam64}&zly_wybor={(int)FailType}&ver={_apiVersion}";
#if DEBUG
        await Server.NextFrameAsync(() => Server.PrintToConsole($"SendPlayerUpdate: {query}"));
#endif

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Valve/CSS Client");

        try
        {
            await httpClient.GetAsync(query);
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred during API SendPlayerUpdate request", ex);
        }
    }
}
