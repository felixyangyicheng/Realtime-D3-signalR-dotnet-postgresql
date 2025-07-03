// Tests/Services/RealtimeLogRepositoryTests.cs
using Xunit;
using Moq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RealTime_D3.Hubs;
using RealTime_D3.Models;
using RealTime_D3.Services;
using Newtonsoft.Json;
using System.Reflection;
using Npgsql;
using System.IO;

namespace RealTime_D3.Tests.Services
{
    public class RealtimeLogRepositoryTests
    {
        [Fact]
        public void LogNotificationHelper_DeserializesPayloadAndInvokesSignalR()
        {
            // Arrange
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<LogHub>>();
            mockHubContext.Setup(ctx => ctx.Clients).Returns(mockClients.Object);

            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("Host=localhost;Database=test;Username=user;Password=pass");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(cfg => cfg.GetSection("ConnectionStrings:postgresql"))
                      .Returns(mockSection.Object);

            var repo = new RealtimeLogRepository(mockConfig.Object, mockHubContext.Object);

            var payload = JsonConvert.SerializeObject(new TbllogInfo
            {
                table = "tbllog",
                action = "UPDATE",
                data = new Tbllog { Id = 1, Value = 123 }
            });

            var ctor = typeof(NpgsqlNotificationEventArgs)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(string), typeof(string) }, null);
            var args = (NpgsqlNotificationEventArgs)ctor!.Invoke(new object[] { 1, "lastlogchange", payload });

            using var sw = new StringWriter();
            Console.SetOut(sw);

            var method = typeof(RealtimeLogRepository)
                .GetMethod("LogNotificationHelper", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            method!.Invoke(repo, new object[] { null!, args });

            // Assert
            mockClientProxy.Verify(proxy => proxy.SendAsync("refreshLog", It.Is<Tbllog>(l => l.Value == 123), default), Times.Once);
            Assert.Contains("Changed", sw.ToString());
        }
    }
}
