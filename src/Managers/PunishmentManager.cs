using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Core.Managers;
public class PunishmentManager
{
    string BanCommand { get; set; }
    string KickCommand { get; set; }

    PunishmentManager(string banCommand, string kickCommand)
    {
        BanCommand = banCommand;
        KickCommand = kickCommand;
    }


    void BanPlayer(CCSPlayerController player, string reason, int timeMinutes)
    {
        string command = FormatCommand(
            commandTemplate: BanCommand,
            player: player,
            reason: reason,
            timeMinutes: timeMinutes);
        Server.ExecuteCommand(command);
    }

    void KickClient(CCSPlayerController player, string reason)
    {
        string command = FormatCommand(
            commandTemplate: KickCommand,
            player: player,
            reason: reason);
        Server.ExecuteCommand(command);
    }

    private static string FormatCommand(string commandTemplate, CCSPlayerController player, string reason = "", int timeMinutes = 0)
    {
        return commandTemplate
            .Replace("{PLAYER_NAME}", player!.PlayerName)
            .Replace("{PLAYER_USERID}", player!.UserId!.Value.ToString())
            .Replace("{PLAYER_IP}", player!.IpAddress!.Split(":")[0])
            .Replace("{REASON}", reason)
            .Replace("{TIME_MINUTES}", timeMinutes.ToString());
    }
}