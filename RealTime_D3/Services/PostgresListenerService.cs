using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using RealTime_D3.Models;
using RealTime_D3.Hubs;

namespace RealTime_D3.Services
{
    public interface IPostgresListenerService : IHostedService
    {
        // Interface unifiée pour le service
    }

    public class PostgresListenerService : IPostgresListenerService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<LogHub> _hubContext;
        private NpgsqlConnection? _connection;
        private CancellationTokenSource? _cts;
        private Task? _listeningTask;

        public PostgresListenerService(
            IConfiguration configuration,
            IHubContext<LogHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _listeningTask = ListenForNotifications(_cts.Token);

            return _listeningTask.IsCompleted ? _listeningTask : Task.CompletedTask;
        }

        private async Task ListenForNotifications(CancellationToken cancellationToken)
        {
            var connectionString = _configuration.GetConnectionString("postgresql")
                ?? throw new NullReferenceException("Connection string 'postgresql' not configured");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (_connection = new NpgsqlConnection(connectionString))
                    {
                        await _connection.OpenAsync(cancellationToken);
                        _connection.Notification += HandlePgNotification;

                        using var cmd = new NpgsqlCommand("LISTEN lastlogchange;", _connection);
                        await cmd.ExecuteNonQueryAsync(cancellationToken);

                        Console.WriteLine("PostgreSQL listener started");

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            try
                            {
                                await _connection.WaitAsync(cancellationToken);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database listener error: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private async void HandlePgNotification(object sender, NpgsqlNotificationEventArgs e)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<TbllogInfo>(e.Payload);
                if (payload?.data != null)
                {
                    await _hubContext.Clients.All.SendAsync("RefreshLogFromHub", payload.data);
                    Console.WriteLine($"Notification sent: {payload.data.Detail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing notification: {ex.Message}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts?.Cancel();

            if (_listeningTask != null)
            {
                await Task.WhenAny(_listeningTask,
                    Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }


}