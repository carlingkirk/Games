using GameAssistant.Interfaces;
using GameAssistant.Models;
using GameAssistant.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Games
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<Program>()
                .Build();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .AddSingleton<IConfiguration>(configuration)
                .AddTransient<IGameStateProvider, GameStateProvider>()
                .AddTransient<ITurnTracker, CustomTurnTracker>()
                .AddTransient<GameStateService>()
                .BuildServiceProvider();

            var gameStateService = serviceProvider.GetService<GameStateService>();
            var gameState = new GameState();
            Console.WriteLine("Enter the name of the game, e.g. Monopoly.");
            gameState.GameName = Console.ReadLine();
            Console.WriteLine("Enter the title of the game, e.g. Friday Night with the Kiddos.");
            gameState.Title = Console.ReadLine();

            var playerCount = 1;
            gameState.Players = new List<IPlayer>();

            Console.WriteLine($"Press any key to start entering your players.");

            while (Console.ReadKey().Key != ConsoleKey.X)
            {
                Console.WriteLine();
                var player = new CustomPlayer();
                Console.WriteLine($"Enter the name of player {playerCount}");
                player.Name = Console.ReadLine();
                gameState.Players.Add(player);

                Console.WriteLine($"Press 'X' if you are finished adding players, any other key to add another.");
                playerCount++;
            }

            await gameStateService.CreateAsync(gameState);

            var turnTracker = serviceProvider.GetService<ITurnTracker>();

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Console.WriteLine();
                turnTracker.TakeTurn(gameState);

                Console.WriteLine($"Press 'Q' to quit, any other key to end your turn.");
            }

            await gameStateService.UpdateGameState(gameState);

            Console.WriteLine($"Your game has been uploaded to the cloud. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
