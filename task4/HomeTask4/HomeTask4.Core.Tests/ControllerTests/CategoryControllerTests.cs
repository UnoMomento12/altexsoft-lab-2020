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
        private Mock<IRepository> _mockRepository;

        public CategoryControllerTests() 
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CategoryController>>();
            _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _categoryController = new CategoryController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact ( DisplayName = "TryCreateCategoryAsync method throws ArgumentException on empty name") ]
        public async Task TryCreateCategoryAsync_Throws_Exception_On_Empty_Name()
        {
            //Arrange
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.TryCreateCategoryAsync("", 1));
            //Assert
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
            Category categoryToTest = new Category { Id = 3, Name = "First set", ParentId = null };
            List<Category> mockDB = GetCategories();
            _mockRepository.Setup(r => r.AddAsync<Category>(categoryToTest)).Callback((Category passedCategory) => mockDB.Add(passedCategory));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, categoryToTest.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == categoryToTest.ParentId));
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(categoryToTest.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.TryCreateCategoryAsync(categoryToTest));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Theory(DisplayName = "TryCreateCategoryAsync method returns bool flag on completion")]
        [InlineData(3, "Soups", 1, false, true)]
        [InlineData(3, "Soups", 1, true, false)]
        public async Task TryCreateCategoryAsync_Returns_Bool(int id, string name, int? parentId, bool isProblem, bool expectedResult)
        {
            //Arrange
            Category categoryToTest = new Category { Id = id, Name = name, ParentId = parentId };
            List<Category> mockDB = GetCategories();
            if (!isProblem)
            {
                _mockRepository.Setup(r => r.AddAsync<Category>(categoryToTest)).Callback((Category passedCategory) => {
                    mockDB.Add(passedCategory);
                });
            }
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.ParentId == parentId));
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var actualResult = await _categoryController.TryCreateCategoryAsync(categoryToTest);
            //Assert
            _mockRepository.Verify(r => r.AddAsync<Category>(categoryToTest), Times.Once);
            Assert.True(actualResult == expectedResult);
        }

        [Fact]
        public async Task TryCreateCategoryAsync_Doesnt_Reach_Exception_Throws_And_Returns_False_On_Null_Reference()
        {
            //Arrange
            Category categoryToTest = null;
            //Act
            var actualResult =  await _categoryController.TryCreateCategoryAsync(categoryToTest);
            //Assert
            Assert.False(actualResult);
        }
        private static List<Category> GetCategories()
        {
            return new List<Category>()
            {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
        }
    }
}
