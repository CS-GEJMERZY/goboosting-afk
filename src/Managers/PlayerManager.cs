using Core.Models;
using CounterStrikeSharp.API.Core;

namespace Core.Managers;

public class PlayerManager
{
    public static PlayerWebInputData GetPlayerWebInputData(CCSPlayerController player)
    {
        var stats = player!.ActionTrackingServices!.MatchStats;

        PlayerWebInputData output = new()
        {
            PlayerName = player.PlayerName,
            Steam64 = player!.AuthorizedSteamID!.SteamId64.ToString(),
            Kills = stats.Kills,
            Deaths = stats.Deaths,
            Assists = stats.Assists,
        };

        return output;
    }

    public static bool IsValid(CCSPlayerController? player)
    {
        return player != null &&
            player.IsValid &&
            !player.IsBot &&
            !player.IsHLTV &&
            player.AuthorizedSteamID != null &&
            player.Connected == PlayerConnectedState.PlayerConnected;
    }
}