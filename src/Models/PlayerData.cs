namespace Core.Models;

public class PlayerData
{
    public bool MenuActive { get; set; } = false;
    public DateTime MenuIssuedTime { get; set; } = DateTime.UnixEpoch;
    public int RandomNumber { get; set; } = -1;
}

