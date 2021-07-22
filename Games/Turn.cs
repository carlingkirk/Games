using System.Collections.Generic;

namespace Games
{
    public class Turn
    {
        public IList<TurnAction> Actions { get; set; } = new List<TurnAction>();
    }
}
