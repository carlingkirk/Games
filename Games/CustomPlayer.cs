using GameAssistant.Interfaces;
using GameAssistant.Models;
using System.Collections.Generic;

namespace Games
{
    public class CustomPlayer : Player, IPlayer
    {
        public IList<Turn> Turns { get; set; } = new List<Turn>();
    }
}
