using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Core.Managers;

public class PunishmentManager(string banCommand, string kickCommand)
{
    string BanCommand { get; set; } = banCommand;
    string KickCommand { get; set; } = kickCommand;

    public string BanPlayer(CCSPlayerController player, int timeMinutes, string reason)
    {
        string command = FormatCommand(
            commandTemplate: BanCommand,
            player: player,
            reason: reason,
            timeMinutes: timeMinutes);

        Server.ExecuteCommand(command);

        return command;
    }

    public string KickPlayer(CCSPlayerController player, string reason)
    {
        string command = FormatCommand(
            commandTemplate: KickCommand,
            player: player,
            reason: reason);

        Server.ExecuteCommand(command);

        return command;
    }

    private static string FormatCommand(string commandTemplate, CCSPlayerController player, string reason = "", int timeMinutes = 0)
    {
        return commandTemplate
            .Replace("{NICK}", player!.PlayerName)
            .Replace("{USERID}", player!.UserId!.Value.ToString())
            .Replace("{ADRES_IP}", player!.IpAddress!.Split(":")[0])
            .Replace("{POWOD}", reason)
            .Replace("{CZAS_W_MINUTACH}", timeMinutes.ToString());
    }
}
