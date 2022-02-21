using Microsoft.AspNetCore.SignalR;
using Npgsql;
using RealTime_D3.Contracts;
using RealTime_D3.Hubs;
using RealTime_D3.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace RealTime_D3.Services
{
    public class RealtimeLogRepository : IRealtimeLogRepository
    {

        private readonly IHubContext<LogHub> _context;
        string connectionString = "";
        public RealtimeLogRepository(IConfiguration configuration,
                                    IHubContext<LogHub> context)
        {
            connectionString = configuration.GetConnectionString("postgresql");
            _context = context;
        }
        public async Task GetLastLog()
        {
            var builder = WebApplication.CreateBuilder();

            await using var con = new NpgsqlConnection(connectionString);
            await con.OpenAsync();
            con.Notification += LogNotificationHelper;
            await using (var cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "LISTEN lastlogchange;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }

            while (true)
            {
                // Waiting for Event
                con.Wait();
            }
        }
        private void LogNotificationHelper(object sender, NpgsqlNotificationEventArgs e)
        {
            //Deserialize Payload Data 
            var dataPayload = JsonConvert.DeserializeObject<tbllogInfo>(e.Payload);
            Console.WriteLine("{0}", dataPayload.table + " :: " + dataPayload.action + " :: " + dataPayload.data.Value);
            _context.Clients.All.SendAsync("refreshLog");

            //Notify Client using SignalR
        }
    }
    public class tbllogInfo
    {
        public string table { get; set; }
        public string action { get; set; }
        public tbllog data { get; set; }
    }
}
