using Core.Models;
using CounterStrikeSharp.API.Core;

namespace Core.Managers;

public class PlayerManager
{
    public static PlayerWebInputData GetPlayerWebInputData(CCSPlayerController player)
    {
        PlayerWebInputData output = new();
        output.PlayerName = player.PlayerName;
        output.Steam64 = player!.AuthorizedSteamID!.SteamId64.ToString();



        var stats = player!.ActionTrackingServices!.MatchStats;
        output.Kills = stats.Kills;
        output.Deaths = stats.Deaths;
        output.Assists = stats.Assists;

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