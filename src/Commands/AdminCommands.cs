using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Core;

public partial class Plugin
{
    [RequiresPermissions("@css/root")]
    [ConsoleCommand("css_gb_testafk", "")]
    public void OnDebugCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        player!.PrintToChat(Localizer["check.checked_players"]);
        Task.Run(ProcessPluginLogic);
    }
}
