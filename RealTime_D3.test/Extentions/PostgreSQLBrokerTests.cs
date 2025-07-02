// Tests/Extensions/PostgreSQLBrokerTests.cs
using Xunit;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using RealTime_D3.Extensions;
using RealTime_D3.Models;
using System.Reflection;

namespace RealTime_D3.Tests.Extensions
{
    public class PostgreSQLBrokerTests
    {
        [Fact]
        public void LogNotificationHelper_ParsesValidJsonAndLogs()
        {
            // Arrange
            var payload = JsonConvert.SerializeObject(new TbllogInfo
            {
                table = "tbllog",
                action = "INSERT",
                data = new Tbllog { Value = 123 }
            });

            var broker = (PostgreSQLBroker)Activator.CreateInstance(
                typeof(PostgreSQLBroker),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                null,
                null
            )!;

            var method = typeof(PostgreSQLBroker)
                .GetMethod("LogNotificationHelper", BindingFlags.NonPublic | BindingFlags.Instance);

            var ctor = typeof(NpgsqlNotificationEventArgs)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(string), typeof(string) }, null);

            var args = (NpgsqlNotificationEventArgs)ctor!.Invoke(new object[] { 1, "logchange", payload });

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            method!.Invoke(broker, new object[] { null!, args });

            var output = sw.ToString().Trim();

            // Assert
            Assert.Contains("tbllog", output);
            Assert.Contains("INSERT", output);
            Assert.Contains("123", output);
        }

    }
}
