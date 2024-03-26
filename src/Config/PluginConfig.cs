using System.Text.Json.Serialization;
using Core.Models;
using CounterStrikeSharp.API.Core;

namespace Core;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("KluczApi")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("TypReakcji")]
    public FailReactionType FailReactionType { get; set; } = FailReactionType.NOTHING;

    [JsonPropertyName("BanKomenda")]
    public string BanCommand { get; set; } = "css_ban {PLAYER_USERID} {REASON} {TIME_MINUTES}";

    [JsonPropertyName("KickKomenda")]
    public string KickCommand { get; set; } = "css_kick {PLAYER_USERID} {REASON}";

    //[JsonPropertyName("MenuDelayMin")]
    public int MenuDelayMin { get; set; } = 300;

    //[JsonPropertyName("MenuDelayMax")]
    public int MenuDelayMax { get; set; } = 600;
}




