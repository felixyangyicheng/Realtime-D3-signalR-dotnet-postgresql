using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;
using RealTime_D3.Models;

namespace RealTime_D3.Hubs
{
    public class LogHub : Hub
    {
        private readonly string _connectionString;
        private NpgsqlConnection? _connection;

        public LogHub(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("postgresql")
                ?? throw new NullReferenceException("Connection string 'postgresql' not configured");
        }

        public override async Task OnConnectedAsync()
        {
            await StartDatabaseListener();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await StopDatabaseListener();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task StartDatabaseListener()
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
            _connection.Notification += HandlePgNotification;

            using var cmd = new NpgsqlCommand("LISTEN lastlogchange;", _connection);
            await cmd.ExecuteNonQueryAsync();

            // Démarrer une tâche d'écoute en arrière-plan
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await _connection.WaitAsync();
                }
            });
        }

        private async Task StopDatabaseListener()
        {
            if (_connection != null)
            {
                _connection.Notification -= HandlePgNotification;
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

        private async void HandlePgNotification(object sender, NpgsqlNotificationEventArgs e)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<TbllogInfo>(e.Payload);
                if (payload?.data != null)
                {
                    // Envoyer à tous les clients connectés
                    await Clients.All.SendAsync("RefreshLogFromHub", payload.data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing notification: {ex.Message}");
            }
        }
    }

    public class TbllogInfo
    {
        public string table { get; set; } = "";
        public string action { get; set; } = "";
        public Tbllog data { get; set; } = new();
    }
}