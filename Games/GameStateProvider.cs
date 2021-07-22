using Azure.Storage.Blobs;
using GameAssistant.Interfaces;
using GameAssistant.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Games
{
    public class GameStateProvider : IGameStateProvider
    {
        private readonly string _blobConnectionString;

        public GameStateProvider(IConfiguration configuration)
        {
            _blobConnectionString = configuration["BlobConnectionString"];
        }

        public async Task<GameState> CreateAsync(GameState state)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, state);
            stream.Position = 0;

            var blobContainer = new BlobContainerClient(_blobConnectionString, "games");
            await blobContainer.CreateIfNotExistsAsync();

            await blobContainer.UploadBlobAsync(state.Id.ToString(), stream);

            return state;
        }

        public async Task<GameState> GetAsync(Guid id)
        {
            var blobClient = new BlobClient(_blobConnectionString, "games", id.ToString());

            var response = await blobClient.DownloadContentAsync();

            return response.Value.Content.ToObjectFromJson<GameState>();
        }

        public async Task SaveAsync(GameState state)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, state);
            stream.Position = 0;

            var blobContainer = new BlobContainerClient(_blobConnectionString, "games");

            var blobClient = blobContainer.GetBlobClient(state.Id.ToString());

            await blobClient.UploadAsync(stream, true);
        }
    }
}
