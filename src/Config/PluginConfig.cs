using System.Text.Json.Serialization;
using Core.Models;
using CounterStrikeSharp.API.Core;

namespace Core.Config
{
    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("KluczApi")]
        public string ApiKey { get; set; } = string.Empty;

        [JsonPropertyName("TypReakcji")]
        public FailReactionType FailReactionType { get; set; } = FailReactionType.NOTHING;

        [JsonPropertyName("BanKomenda")]
        public string BanCommand { get; set; } = "css_ban #{USERID} {CZAS_W_MINUTACH} {POWOD}";
        [JsonPropertyName("BanCzasMinuty")]
        public int BanTimeMinutes { get; set; } = 5;

        [JsonPropertyName("BanPowod")]
        public string BanReason { get; set; } = "GOBOOSTING AFK";

        [JsonPropertyName("KickKomenda")]
        public string KickCommand { get; set; } = "css_kick #{USERID} {POWOD}";

        [JsonPropertyName("KickPowod")]
        public string KickReason { get; set; } = "GOBOOSTING AFK";

        [JsonPropertyName("CzasMenuAfk")]
        public int MenuMaxWaitTime { get; set; } = 30;
    }
}

