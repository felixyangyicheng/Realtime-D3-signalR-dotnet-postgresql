// Tests/Services/BaseRepositoryTests.cs
using Xunit;
using Microsoft.EntityFrameworkCore;
using RealTime_D3.Models;
using RealTime_D3.Services;
using AutoMapper;
using Moq;
using System.Threading.Tasks;
using System.Linq;

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

            var mapperMock = new Mock<IMapper>();
            _repository = new BaseRepository<Tbllog>(_context, mapperMock.Object);
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            var log = new Tbllog { Id = 1, Value = 5315 };
            var result = await _repository.AddAsync(log);

            Assert.NotNull(result);
            Assert.Equal(5315, result.Value);
            Assert.Single(_context.Tbllogs);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAll()
        {
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 2, Value = 456 });
            await _context.SaveChangesAsync();

            var list = await _repository.GetAllAsync();

            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task GetAsync_ReturnsEntity()
        {
            var log = new Tbllog { Id = 3, Value = 456 };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            var fetched = await _repository.GetAsync(3);

            Assert.NotNull(fetched);
            Assert.Equal(456, fetched.Value);
        }

        [Fact]
        public async Task Exists_ReturnsTrue_WhenFound()
        {
            await _context.Tbllogs.AddAsync(new Tbllog { Id = 4, Value = 654 });
            await _context.SaveChangesAsync();

            var exists = await _repository.Exists(4);

            Assert.True(exists);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            var log = new Tbllog { Id = 5, Value = 987 };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(5);

            Assert.Empty(_context.Tbllogs);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            var log = new Tbllog { Id = 6, Value = 988 , Detail= "Old" };
            await _context.Tbllogs.AddAsync(log);
            await _context.SaveChangesAsync();

            log.Detail = "Updated";
            await _repository.UpdateAsync(log);

            var updated = await _context.Tbllogs.FindAsync(6);
            Assert.Equal("Updated", updated!.Detail);
        }
    }
}