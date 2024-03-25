using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Core;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("KluczApi")]
    public string ApiKey { get; set; } = string.Empty;

    //[JsonPropertyName("MenuDelayMin")]
    public int MenuDelayMin { get; set; } = 300;

    //[JsonPropertyName("MenuDelayMax")]
    public int MenuDelayMax { get; set; } = 600;
}




