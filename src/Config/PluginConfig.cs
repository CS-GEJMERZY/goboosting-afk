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
        public string BanCommand { get; set; } = "css_ban #{PLAYER_USERID} {TIME_MINUTES} {REASON} ";
        [JsonPropertyName("BanCzasMinuty")]
        public int BanTimeMinutes { get; set; } = 5;

        [JsonPropertyName("BanPowod")]
        public string BanReason { get; set; } = "GO-BOOSTING AFK";

        [JsonPropertyName("KickKomenda")]
        public string KickCommand { get; set; } = "css_kick #{PLAYER_USERID} {REASON}";

        [JsonPropertyName("KickPowod")]
        public string KickReason { get; set; } = "GO-BOOSTING AFK";

        [JsonPropertyName("CzasMenuAfk")]
        public int MenuMaxWaitTime { get; set; } = 15;
    }
}

