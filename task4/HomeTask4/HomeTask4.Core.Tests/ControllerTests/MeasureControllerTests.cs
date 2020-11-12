using HomeTask4.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using HomeTask4.Core.Controllers;
using Microsoft.Extensions.Logging;
using HomeTask4.Core.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace HomeTask4.Core.Tests.ControllerTests
{
    public class MeasureControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private MeasureController _measureController;
        private Mock<ILogger<MeasureController>> _loggerMock;
        private Mock<IRepository> _mockRepository;

        public MeasureControllerTests() 
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<MeasureController>>();
            _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _measureController = new MeasureController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact ( DisplayName = "CreateMeasureAsync method throws ArgumentException on name null reference") ]
        public async Task CreateMeasureAsync_Throws_Exception_On_Name_Null()
        {
            //Arrange
            Measure toCreate = new Measure { Name = null };
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _measureController.CreateMeasureAsync(toCreate));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("name is null!", caughtException.Message);
        }

        [Fact]
        public async Task CreateMeasureAsync_Throws_ArgumentException_On_Null_Reference()
        {
            //Arrange
            Measure measureToTest = null;
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _measureController.CreateMeasureAsync(measureToTest));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("reference is null", caughtException.Message);
        }

        [Fact(DisplayName = "CreateMeasureAsync method throws ArgumentException if measure already exists")]
        public async Task CreateMeasure_Throws_ArgumentException_If_Measure_Exists()
        {
            //Arrange
            Measure measureToTest = new Measure { Id = 15, Name = "g"};
            List<Measure> mockDB = GetMeasures();
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, measureToTest.Name, StringComparison.OrdinalIgnoreCase)));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _measureController.CreateMeasureAsync(measureToTest));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Fact]
        public async Task CreateMeasureAsync_Creates_Measure()
        {
            //Arrange
            Measure measureToTest = new Measure { Id = 15, Name = "tea spoon" };
            List<Measure> mockDB = GetMeasures();
            _mockRepository.Setup(r => r.AddAsync<Measure>(measureToTest)).Callback((Measure passedMeasure) => {
                mockDB.Add(passedMeasure);
            });
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, measureToTest.Name, StringComparison.OrdinalIgnoreCase)));
            //Act
            await _measureController.CreateMeasureAsync(measureToTest);
            //Assert
            var retrieved = mockDB.FirstOrDefault(x => x.Name == measureToTest.Name);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Measure>(measureToTest), Times.Once);
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task DeleteMeasureByIdAsync_Should_Delete_Measure()
        {
            //Arrange
            List<Measure> mockMeasureDB = GetMeasures();
            int id = 2;
            _mockRepository.Setup(r => r.GetByIdAsync<Measure>(id)).ReturnsAsync((int a) => mockMeasureDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.DeleteAsync<Measure>(It.Is<Measure>(entity => entity.Id == id)))
                .Callback((Measure measure) =>
                {
                    mockMeasureDB.Remove(measure);
                });
            //Act
            await _measureController.DeleteMeasureByIdAsync(id);
            //Assert
            var mustBeNull = mockMeasureDB.FirstOrDefault(x => x.Id == id);
            _mockRepository.Verify(r => r.GetByIdAsync<Measure>(id), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync<Measure>(It.Is<Measure>(entity => entity.Id == id)), Times.Once);
            Assert.Null(mustBeNull);
        }
        [Fact]
        public async Task UpdateMeasureAsync_Should_Update()
        {
            //Arrange
            List<Measure> mockMeasureDB = GetMeasures();
            Measure toUpdate = new Measure { Id = 1, Name = "gram" };
            _mockRepository.Setup(r => r.GetByIdAsync<Measure>(toUpdate.Id)).ReturnsAsync((int a) => mockMeasureDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.UpdateAsync<Measure>(It.Is<Measure>(entity => entity.Id == toUpdate.Id)))
                .Callback((Measure recipe) =>
                {
                    recipe.Name = toUpdate.Name;
                });
            //Act
            await _measureController.UpdateMeasureAsync(toUpdate);
            //Assert
            var retrieved = mockMeasureDB.FirstOrDefault(x => x.Id == toUpdate.Id);
            _mockRepository.Verify(r => r.GetByIdAsync<Measure>(toUpdate.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync<Measure>(It.Is<Measure>(entity => entity.Id == toUpdate.Id)), Times.Once);
            Assert.Contains("gr", retrieved.Name);
        }
        private static List<Measure> GetMeasures()
        {
            return new List<Measure>()
            {
                new Measure { Id = 1 , Name = "g"},
                new Measure { Id = 2 , Name = "kg"},
                new Measure { Id = 3 , Name = "spoon"}
            };
        }
    }
}
