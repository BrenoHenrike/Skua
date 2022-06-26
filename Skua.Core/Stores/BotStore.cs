using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

namespace Skua.Core.Stores;
public class BotStore
{
    private readonly IScriptPlayer Player;

    public event Action LoggedInChanged;
    public bool LoggedIn => Player.LoggedIn;

    public BotStore(IScriptPlayer player)
    {
        Player = player;
    }
}
