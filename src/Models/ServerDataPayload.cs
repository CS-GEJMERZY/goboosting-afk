﻿namespace Core.Models;

public class ServerDataPayload
{
    public string Hostname { get; set; } = "invalid";
    public int MaxPlayers { get; set; } = 0;
    public int PlayerCount { get; set; } = 0;
    public string MapName { get; set; } = "invalid";
    public bool Password { get; set; } = false;
    public string ModDesc { get; set; } = "Counter-Strike 2";
}

