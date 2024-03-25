using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace Core;

public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "goboosting-afk";
    public override string ModuleAuthor => "Hacker";
    public override string ModuleVersion => "1.0.0";

    public required PluginConfig Config { get; set; }


    //internal Dictionary<CCSPlayerController, Models.PlayerData> PlayerCache = new();

    internal static ILogger? _logger;

    public void OnConfigParsed(PluginConfig _Config)
    {
        Config = _Config;
    }

    public override void Load(bool hotReload)
    {
        _logger = Logger;

        if (hotReload)
        {

        }
    }

    public override void Unload(bool hotReload)
    {

    }
}
