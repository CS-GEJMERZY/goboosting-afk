﻿using Core.Config;
using Core.Managers;
using Core.Models;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

namespace Core;

public partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "goboosting-afk";
    public override string ModuleAuthor => "Hacker";
    public override string ModuleVersion => "0.0.10";

    public required PluginConfig Config { get; set; }

    internal Dictionary<CCSPlayerController, PlayerData> _playerCache = [];
    private WebManager? WebManager { get; set; }
    private PunishmentManager? PunishmentManager { get; set; }
    private Timer? LogicTimer { get; set; }
    private Timer? ServerDataTimer { get; set; }
    private int MenuDelayMin { get; set; } = 50;
    private int MenuDelayMax { get; set; } = 55;

    private readonly string PluginPrefix = $"{ChatColors.Red}# {ChatColors.Grey}GO{ChatColors.White}BOOSTING";

    string ServerIp = string.Empty;

    public void OnConfigParsed(PluginConfig _Config)
    {
        Config = _Config;

        if (Config.MenuMaxWaitTime < 20)
        {
            Config.MenuMaxWaitTime = 20;
            Logger.LogInformation("MenuMaxWaitTime must be equal or greater than 20.");
        }

        WebManager = new(apiKey: Config.ApiKey);
        PunishmentManager = new(banCommand: Config.BanCommand, kickCommand: Config.KickCommand);
    }

    public override void Load(bool hotReload)
    {
        LogicTimer = new Timer(callback: ProcessTimerCallback!, null, GetRandomInterval() * 1000, GetRandomInterval() * 1000);
        ServerDataTimer = new Timer(callback: ServerDataTimer_Callback!, null, 20 * 1000, 5 * 60 * 1000);

        RegisterListener<OnTick>(() =>
        {
            foreach (var player in Utilities.GetPlayers().Where(x => PlayerManager.IsValid(x)).ToList())
            {
                if (!_playerCache.ContainsKey(player))
                {
                    continue;
                }

                OnTick(player);
            }
        });

        AddCommandListener("say", OnClientSay);
        AddCommandListener("say_team", OnClientSay);

        if (hotReload)
        {
            foreach (var player in Utilities.GetPlayers())
            {
                if (!PlayerManager.IsValid(player))
                {
                    continue;
                }

                if (!_playerCache.ContainsKey(player))
                {
                    _playerCache.Add(player, new PlayerData());
                }
            }
        }

        // check if works
        AddTimer(5.0f, async () =>
        {
            await Server.NextFrameAsync(() => ServerIp = GetServerIp());
            Logger.LogInformation("Uruchomiono plugin na serwerze {ServerIp}", ServerIp);

        });
    }

    public override void Unload(bool hotReload)
    {
        LogicTimer?.Dispose();
        ServerDataTimer?.Dispose();
    }

    private int GetRandomInterval()
    {
        return MenuDelayMin + new Random().Next(MenuDelayMax - MenuDelayMin);
    }

    public async void ServerDataTimer_Callback(object state)
    {
        ServerDataPayload data = new();

        await Server.NextFrameAsync(() =>
        {
            ConVar? hostnameCvar = ConVar.Find("hostname");
            if (hostnameCvar != null)
            {
                data.Hostname = hostnameCvar.StringValue;
            }

            data.MapName = Server.MapName;
            data.MaxPlayers = Server.MaxPlayers;

            ConVar? passwordCvar = ConVar.Find("sv_password");
            if (passwordCvar != null)
            {
                data.Password = ConVar.Find("sv_password")!.StringValue.Length > 1;
            }

            data.PlayerCount = Utilities.GetPlayers()
                        .Count(player => PlayerManager.IsValid(player) && !player.IsBot && !player.IsHLTV);
        });

        await WebManager!.SendServerData(ServerIp, data);
    }

    private async void ProcessTimerCallback(object state)
    {
        await ProcessPluginLogic();
        int newInterval = GetRandomInterval();

        LogicTimer!.Change(newInterval * 1000, Timeout.Infinite);
    }

    private async Task ProcessPluginLogic()
    {
        List<PlayerWebInputData> inputData = [];
        ManualResetEventSlim eventSlim = new(false);

        Server.NextFrame(() =>
        {
            inputData = Utilities.GetPlayers()
                             .Where(player => PlayerManager.IsValid(player))
                             .Select(player => PlayerManager.GetPlayerWebInputData(player))
                             .ToList();
            eventSlim.Set();
        });
        eventSlim.Wait();

        if (inputData.Count == 0)
        {
            return;
        }

        List<PlayerWebResponseData>? responseData = null;
        try
        {
            responseData = await WebManager!.GetPlayersAsync(inputData, ServerIp);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to fetch boosters. Error: {}", ex);
            return;
        }

        if (responseData == null)
        {
            return;
        }

        foreach (var response in responseData)
        {

            if (!ulong.TryParse(response.Steam64, out ulong steam64))
            {
                Logger.LogWarning("Player has incorrect steam64 from query: {steamid64}", response.Steam64);
                continue;
            }

            CCSPlayerController? player = null;

            await Server.NextFrameAsync(() =>
            {
                player = Utilities.GetPlayerFromSteamId(steam64);
                eventSlim.Set();
            });
            eventSlim.Wait();

            await Server.NextFrameAsync(() =>
            {
                if (!PlayerManager.IsValid(player) ||
                    !_playerCache.ContainsKey(player!) ||
                    response.DoTest == 0)
                {
                    return;
                }

                ProcessPlayerMenu(player!);
            });
        }
    }

    private void ProcessPlayerMenu(CCSPlayerController player)
    {
        var playerData = _playerCache[player!];
        if (!playerData.MenuActive)
        {
            playerData.MenuActive = true;
            playerData.MenuIssuedTime = DateTime.Now;
            playerData.RandomNumber = new Random().Next(1, 3);
        }

        player!.PrintToChat($"{PluginPrefix} {Localizer["check.begin"]}");
    }

    private void ProcessPlayerPunishment(CCSPlayerController player)
    {
        switch (Config.FailReactionType)
        {
            case FailReactionType.KICK:
                {
                    Logger.LogInformation("Wyrzucono gracza {PlayerName}(steamid64: {SteamId64}): minął czas na reakcje w menu.",
                        player?.PlayerName, player?.AuthorizedSteamID?.SteamId64);

                    PunishmentManager!.KickPlayer(player!, Config.KickReason);
                    break;
                }
            case FailReactionType.BAN:
                {
                    Logger.LogInformation("Zbanowano gracza {PlayerName}(steamid64: {SteamId64}) na {BanTimeMinutes} minut: minął czas na reakcje w menu.",
                        player?.PlayerName, player?.AuthorizedSteamID?.SteamId64, Config.BanTimeMinutes);
                    PunishmentManager!.BanPlayer(player!, Config.BanTimeMinutes, Config.KickReason);

                    break;
                }
            case FailReactionType.NOTHING:
                {
                    Logger.LogInformation("Graczowi {PlayerName}({SteamId64}) minął czas na reakcje w menu.", player?.PlayerName, player?.AuthorizedSteamID?.SteamId64);
                    break;
                }
        }
    }
}

