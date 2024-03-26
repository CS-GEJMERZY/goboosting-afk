using Core.Models;
using Newtonsoft.Json;


namespace Core.Managers;


public class WebManager
{
    private string ApiKey { get; set; } = "1234567890";

    private int ApiVersion { get; set; } = 200;

    private string ServerIp { get; set; }

    public WebManager(string apiKey, string serverIp)
    {
        ApiKey = apiKey;
        ServerIp = serverIp;
    }

    public async Task<List<PlayerWebResponseData>?> GetPlayersAsync(List<PlayerWebInputData> PlayerInputData)
    {
        var jsonData = new
        {
            json = PlayerInputData.Select((player, index) => new
            {
                steam64 = player.Steam64,
                name = player.PlayerName,
                kill = player.Kills,
                dead = player.Deaths,
                assist = player.Assists
            })
        };
        string jsonString = JsonConvert.SerializeObject(jsonData);

        string query = $"https://goboosting.pl/api.php?gracze&ip={ServerIp}&api={ApiKey}&ver={ApiVersion}&json={jsonString}";
        using HttpClient httpClient = new();

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                List<PlayerWebResponseData> playerData = JsonConvert.DeserializeObject<List<PlayerWebResponseData>>(responseContent);
                return playerData;

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

    public async Task SendPlayerUpdate(PlayerMenuFailType FailType, string Steam64)
    {
        string query = $"https://goboosting.pl/api.php?afk&api={ApiKey}&ip={ServerIp}&steam64={Steam64}&zly_wybor={(int)FailType}&ver={ApiVersion}";

        using HttpClient httpClient = new();

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


