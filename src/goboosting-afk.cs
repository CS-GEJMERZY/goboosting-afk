using Core.Managers;
using Core.Models;
using CounterStrikeSharp.API;
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

    private Timer? LogicTimer { get; set; }

    private int LastTimerInterval { get; set; }

    public void OnConfigParsed(PluginConfig _Config)
    {
        Config = _Config;

    }

    public override void Load(bool hotReload)
    {

        string ServerIp = GetServerIp();
        WebManager = new(Config.ApiKey, ServerIp);
        Logger.LogInformation($"Uruchomiono plugin na serwerze {ServerIp}");

        LastTimerInterval = Config.MenuDelayMin + new Random().Next(Config.MenuDelayMax - Config.MenuDelayMin);
        LogicTimer = new Timer(callback: ExecutePluginLogic!, null, LastTimerInterval, LastTimerInterval);

        if (hotReload)
        {

        }
    }

    public override void Unload(bool hotReload)
    {
        LogicTimer?.Dispose();
    }

    private async void ExecutePluginLogic(object state)
    {
        List<PlayerWebInputData> inputData = [];

        Server.NextFrame(() =>
        {
            inputData = Utilities.GetPlayers()
                             .Where(x => PlayerManager.IsValid(x))
                             .Select(player => PlayerManager.GetPlayerWebInputData(player))
                             .ToList();
        });

        if (inputData.Count == 0)
        {
            return;
        }
        List<PlayerWebResponseData>? playerData = null;
        try
        {
            playerData = await WebManager!.GetPlayersAsync(inputData);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to fetch boosters. ", ex);
            return;
        }

        if (playerData == null) { return; }

        foreach (var playerRespose in playerData)
        {
            ulong steam64 = 0;
            if (!ulong.TryParse(playerRespose.Steam64, out steam64))
            {
                Logger.LogWarning("Player has incorrect steam64 from query: ", playerRespose.Steam64);
                continue;
            }

            var player = Utilities.GetPlayerFromSteamId(steam64);
            if (PlayerManager.IsValid(player))
            {
                await ProcessPlayerMenu(player, playerRespose.CreditsEarned);

            }
        }
    }

    private Task ProcessPlayerMenu(CCSPlayerController player, int CreditsEarned)
    {
        Server.NextFrame(() =>
        {

        });
    }

}
