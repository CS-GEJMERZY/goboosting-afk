using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Cvars;

namespace Core;

public partial class Plugin
{
    public static string GetServerIp()
    {
        string Ip = ConVar.Find("ip")!.StringValue;

        return Ip;
    }
}