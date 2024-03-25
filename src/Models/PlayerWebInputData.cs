namespace Core.Models;

public struct PlayerWebInputData
{
    public string Steam64 { get; set; }
    public string PlayerName { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
}