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
using System.ComponentModel;

namespace HomeTask4.Core.Tests.ControllerTests
{
    public class CategoryControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CategoryController _categoryController;
        private Mock<ILogger<CategoryController>> _loggerMock;

        public CategoryControllerTests() 
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CategoryController>>();
            _categoryController = new CategoryController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact ( DisplayName = "TryCreateCategoryAsync method throws ArgumentException on empty name") ]
        public async Task TryCreateCategoryAsync_Throws_Exception_On_Empty_Name()
        {
            //Arrange
            var mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(mockRepository.Object);
            mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>()));
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>()));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.TryCreateCategoryAsync("", 1));
            //Assert
            mockRepository.Verify(r => r.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
            mockRepository.Verify(r => r.AddAsync<Category>(null), Times.Never);
            mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("null or empty", caughtException.Message);
        }

        [Fact(DisplayName = "TryCreateCategoryAsync method throws ArgumentException if category already exists")]
        public async Task TryCreateCategory_Throws_ArgumentException_If_Category_Exists()
        {
            //Arrange
            Category testedCategory = new Category { Id = 3, Name = "First set", ParentId = null };
            var mockRepository = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(mockRepository.Object);
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, testedCategory.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == testedCategory.ParentId));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(testedCategory.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.TryCreateCategoryAsync(testedCategory));
            //Assert
            mockRepository.Verify(r => r.AddAsync<Category>(testedCategory), Times.Never);
            mockRepository.Verify(r => r.GetByIdAsync<Category>(testedCategory.Id), Times.Never);
            mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Theory(DisplayName = "TryCreateCategoryAsync method returns true bool flag on completion")]
        [InlineData(3, "Soups", 1, true)]
        public async Task TryCreateCategoryAsync_Returns_True(int id, string name, int? parentId, bool result)
        {
            //Arrange
            Category testedCategory = new Category { Id = id, Name = name, ParentId = parentId };
            var mockRepository = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(mockRepository.Object);
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.ParentId == parentId));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var resultFlag = await _categoryController.TryCreateCategoryAsync(testedCategory);
            //Assert
            mockRepository.Verify(r => r.AddAsync<Category>(testedCategory), Times.Once);
            mockRepository.Verify(r => r.GetByIdAsync<Category>(testedCategory.Id), Times.Once);
            mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
            Assert.True(resultFlag == result);
        }

        [Theory(DisplayName = "TryCreateCategoryAsync method returns false bool flag on completion with no exceptions")]
        [InlineData(3, "Soups", 1, false)]
        public async Task TryCreateCategoryAsync_Returns_False(int id, string name, int? parentId, bool result)
        {
            //Arrange
            Category testedCategory = new Category { Id = id, Name = name, ParentId = parentId };
            var mockRepository = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(mockRepository.Object);
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())); // Emulate some problem with DB insert on DB server side
            mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.ParentId == parentId));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var resultFlag = await _categoryController.TryCreateCategoryAsync(testedCategory);
            //Assert
            mockRepository.Verify(r => r.AddAsync<Category>(testedCategory), Times.Once);
            mockRepository.Verify(r => r.GetByIdAsync<Category>(testedCategory.Id), Times.Once);
            mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
            Assert.True(resultFlag == result);
        }


        [Fact(DisplayName = "TryCreateCategoryAsync method doesn't throw ArgumentNullException and returns false")]
        public async Task CreateCategoryAsync_Doesnt_Throw_ArgumentNullException()
        {
            //Arrange
            Category testedCategory = null;
            var mockRepository = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWorkMock.Setup(u => u.Repository).Returns(mockRepository.Object);
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var resultFlag =  await _categoryController.TryCreateCategoryAsync(testedCategory);
            //Assert
            mockRepository.Verify(r => r.AddAsync<Category>(testedCategory), Times.Never);
            mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
            mockRepository.Verify(r => r.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
            Assert.False(resultFlag);
        }
    }
}
