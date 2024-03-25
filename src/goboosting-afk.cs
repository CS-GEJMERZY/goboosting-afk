using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace Core;

public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "goboosting-afk";
    public override string ModuleAuthor => "Hacker";
    public override string ModuleVersion => "1.0.0";

    public required PluginConfig Config { get; set; }

    internal Managers.WebManager? WebManager { get; set; }


    public void OnConfigParsed(PluginConfig _Config)
    {
        Config = _Config;

    }

    public override void Load(bool hotReload)
    {

        string ServerIp = GetServerIp();
        WebManager = new(Config.ApiKey, ServerIp);
        Logger.LogInformation($"Uruchomiono plugin na serwerze {ServerIp}");

        if (hotReload)
        {

        }
    }

    public override void Unload(bool hotReload)
    {

    }
}
