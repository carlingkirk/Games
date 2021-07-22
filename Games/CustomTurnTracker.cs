using GameAssistant.Models;
using GameAssistant.Services;
using System;
using System.Collections.Generic;

namespace Games
{
    public class CustomTurnTracker : TurnTracker
    {
        public override GameState TakeTurn(GameState state)
        {
            Console.WriteLine("Enter a number from 1 - 10.");
            var input = Console.ReadKey();

            var currentPlayer = state.CurrentPlayer as CustomPlayer;
            currentPlayer.Turns.Add(new Turn
            {
                Actions = new List<TurnAction>
                {
                    new TurnAction
                    {
                        Name = "Guess",
                        Details = $"Guessed the number {input}"
                    }
                }
            });

            base.EndTurn(state);

            return state;
        }
    }
}
