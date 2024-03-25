using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Core;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("KluczApi")]
    public string ApiKey { get; set; } = string.Empty;


    //public PluginConfig()
    //{
    //}
}




