using Core.Managers;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace Core;

public partial class Plugin
{
    [GameEventHandler]
    public HookResult EventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;

        if (!PlayerManager.IsValid(player))
        {
            return HookResult.Continue;
        }

        if (!_playerCache.TryGetValue(player!, out Models.PlayerData? playerData))
        {
            playerData = new Models.PlayerData();
            _playerCache.Add(player!, playerData);
        }

        playerData.MenuActive = false;
        playerData.MenuIssuedTime = DateTime.UnixEpoch;

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnClientDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        if (!PlayerManager.IsValid(player))
        {
            return HookResult.Continue;
        }

        _playerCache.Remove(player!);

        return HookResult.Continue;
    }

    private void OnTick(CCSPlayerController player)
    {
        var playerData = _playerCache[player];

        if (!playerData.MenuActive)
        {
            return;
        }

        double timeElapsed = (DateTime.Now - playerData.MenuIssuedTime).TotalSeconds;
        if (timeElapsed > Config.MenuMaxWaitTime)
        {
            // Time for reaction has elapsed
            var steamid64 = player!.AuthorizedSteamID!.SteamId64.ToString();

            Task.Run(async () =>
            {
                await WebManager!.SendPlayerUpdate(Models.PlayerMenuFailType.TimeEnd, steamid64, ServerIp);
            });

            player!.PrintToChat($"{PluginPrefix} {Localizer["fail.time"]}");
            _playerCache[player] = new Models.PlayerData();
            ProcessPlayerPunishment(player);
        }

        double timeLeft = Math.Round(Config.MenuMaxWaitTime - timeElapsed, 2);
        if (timeLeft < 0)
        {
            return;
        }

        string message = $@"<font class='fontSize-l' color='gray'> GoBoosting.pl test AFK</font><br> 
                            <font class='fontSize-l' color='green'>Wpisz</font> 
                            <font class='fontSize-l' color='yellow'> !{playerData.RandomNumber}</font>
                            <font class='fontSize-l' color='green'> na czacie!</font><br>
                            <font class='fontSize-l' color='green'> Pozostało</font>
                            <font class='fontSize-l' color='red'> {timeLeft}</font> 
                            <font class='fontSize-l' color='green'>sekund</font>";

        player!.PrintToCenterHtml(message);
    }

    private HookResult OnClientSay(CCSPlayerController? player, CommandInfo info)
    {
        if (!PlayerManager.IsValid(player) ||
            !_playerCache.TryGetValue(player!, out var playerData))
        {
            return HookResult.Continue;
        }

        if (!playerData.MenuActive)
        {
            return HookResult.Continue;
        }

        string message = info.GetArg(1);

        if (message.StartsWith('!') && int.TryParse(message.AsSpan(1), out int number))
        {
            if (number < 0 || number > 3)
            {
                return HookResult.Continue;
            }

            if (number == playerData.RandomNumber)
            {
                // Input number is correct, disable check

                _playerCache[player!] = new Models.PlayerData();
                player!.PrintToChat($"{PluginPrefix} {Localizer["success.command"]}");
            }
            else
            {
                // Wrong number input, process punishment
                var steam64 = player!.AuthorizedSteamID!.SteamId64.ToString();
                Task.Run(async () =>
                {
                    await WebManager!.SendPlayerUpdate(Models.PlayerMenuFailType.WrongChoice, steam64, ServerIp);
                });

                player!.PrintToChat($"{PluginPrefix} {Localizer["fail.command"]}");
                _playerCache[player!] = new Models.PlayerData();
                ProcessPlayerPunishment(player);
            }

            return HookResult.Handled;
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController? attacker = @event.Attacker;

        if (!PlayerManager.IsValid(attacker) ||
            attacker!.IsBot ||
            !_playerCache.TryGetValue(attacker, out Models.PlayerData? playerData))
        {
            return HookResult.Continue;
        }

        if (playerData.MenuActive)
        {
            attacker!.PrintToChat($"{PluginPrefix} {Localizer["success.killed"]}");
            _playerCache[attacker] = new Models.PlayerData();
        }

        return HookResult.Continue;
    }
}

