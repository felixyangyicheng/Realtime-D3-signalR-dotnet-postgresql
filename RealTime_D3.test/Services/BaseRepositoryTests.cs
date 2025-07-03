// Tests/Services/BaseRepositoryTests.cs
using Xunit;
using Microsoft.EntityFrameworkCore;
using RealTime_D3.Models;
using RealTime_D3.Services;
using AutoMapper;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace RealTime_D3.Tests.Services
{
    public class BaseRepositoryTests
    {
        private readonly RealtimeDbContext _context;
        private readonly BaseRepository<Tbllog> _repository;

        public BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<RealtimeDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new RealtimeDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var mapperMock = new Mock<IMapper>();
            _repository = new BaseRepository<Tbllog>(_context, mapperMock.Object);
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            var log = new Tbllog { Id = 1,Value=123, Detail = "Test" };
            var result = await _repository.AddAsync(log);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Detail);
            Assert.Equal(123, result.Value);
            Assert.Single(_context.Tbllogs);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAll()
        {
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 2, Value = 124,Detail = "V1" });
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 3, Value = 125,Detail = "V2" });
            await _context.SaveChangesAsync();

            var list = await _repository.GetAllAsync();

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetAsync_ReturnsEntity()
        {
            var log = new Tbllog { Id = 4, Value = 123, Detail = "GetMe" };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            var fetched = await _repository.GetAsync(4);

            Assert.NotNull(fetched);
            Assert.Equal("GetMe", fetched.Detail);
            Assert.Equal(123, fetched.Value);
        }

        [Fact]
        public async Task GetAsync_ThrowsIfNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _repository.GetAsync(null));
        }

        [Fact]
        public async Task Exists_ReturnsTrue_WhenFound()
        {
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 5, Detail = "Exists" });
            await _context.SaveChangesAsync();

            var exists = await _repository.Exists(5);

            Assert.True(exists);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_WhenNotFound()
        {
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 5, Detail = "Exists" });
            await _context.SaveChangesAsync();
            var exists = await _repository.Exists(999);
            Assert.False(exists);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            var log = new Tbllog { Id = 6, Value =123, Detail= "DeleteMe" };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(6);

            Assert.Empty(_context.Tbllogs);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            var log = new Tbllog { Id = 7, Value =123, Detail= "Old" };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            log.Detail = "Updated";
            log.Value = 321;
            await _repository.UpdateAsync(log);

            var updated = await _context.Tbllogs.FindAsync(7);
            Assert.Equal("Updated", updated!.Detail);
            Assert.Equal(321, updated!.Value);
        }
    }
}
