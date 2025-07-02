// Tests/Controllers/TbllogControllerTests.cs
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using RealTime_D3.Controllers;
using RealTime_D3.Contracts;
using RealTime_D3.Dtos;
using RealTime_D3.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RealTime_D3.Tests.Controllers
{
    public class TbllogControllerTests
    {
        private readonly Mock<ITbllogRepository> _mockRepo;
        private readonly Mock<IRealtimeLogRepository> _mockRealtime;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TbllogController>> _mockLogger;
        private readonly TbllogController _controller;

        public TbllogControllerTests()
        {
            _mockRepo = new Mock<ITbllogRepository>();
            _mockRealtime = new Mock<IRealtimeLogRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TbllogController>>();

            _controller = new TbllogController(
                _mockLogger.Object,
                _mockRepo.Object,
                _mockRealtime.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetLogRealtime_ReturnsOk()
        {
            _mockRealtime.Setup(r => r.GetLastLog()).Returns(Task.CompletedTask);

            var result = await _controller.GetLogRealtime();

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetLog_ReturnsOkWithMappedDto()
        {
            var id = 1;
            var entity = new Tbllog { Id = id };
            var dto = new TbllogDto { Id = id };

            _mockRepo.Setup(r => r.GetAsync(id)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<TbllogDto>(entity)).Returns(dto);

            var result = await _controller.GetLog(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task PostLog_ReturnsCreatedAtAction()
        {
            var createDto = new TbllogCreateDto();
            var entity = new Tbllog { Id = 1 };

            _mockMapper.Setup(m => m.Map<Tbllog>(createDto)).Returns(entity);

            var result = await _controller.PostLog(createDto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(entity, created.Value);
        }

        [Fact]
        public async Task PutLog_IdMismatch_ReturnsBadRequest()
        {
            var result = await _controller.PutLog(1, new TbllogDto { Id = 2 });
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutLog_NotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetAsync(1)).ReturnsAsync((Tbllog)null);
            var result = await _controller.PutLog(1, new TbllogDto { Id = 1 });
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
