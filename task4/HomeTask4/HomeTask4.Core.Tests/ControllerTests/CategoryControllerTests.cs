﻿using HomeTask4.SharedKernel.Interfaces;
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
using HomeTask4.Core.Exceptions;
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

        [Fact ( DisplayName = "TryCreateCategoryAsync method throws EmptyFieldException on empty name") ]
        public async Task TryCreateCategoryAsync_Throws_Exception_On_Empty_Name()
        {
            //Act
            var caughtException = await Assert.ThrowsAsync<EmptyFieldException>(async () => await _categoryController.TryCreateCategoryAsync("", 1));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("null or empty", caughtException.Message);
        }

        [Fact(DisplayName = "TryCreateCategoryAsync method throws EntryAlreadyExistsException if category already exists")]
        public async Task TryCreateCategory_Throws_ArgumentException_If_Category_Exists()
        {
            //Arrange
            Category categoryToTest = new Category { Id = 3, Name = "First set", ParentId = null };
            List<Category> mockDB = GetCategories();
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, categoryToTest.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == categoryToTest.ParentId));
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryAlreadyExistsException>(async () => await _categoryController.TryCreateCategoryAsync(categoryToTest));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Theory(DisplayName = "TryCreateCategoryAsync method returns bool flag on completion")]
        [InlineData(3, "Soups", 1)]
        [InlineData(4, "Hot Soups", 3)]
        public async Task TryCreateCategoryAsync_Returns_Bool(int id, string name, int? parentId)
        {
            //Arrange
            Category categoryToTest = new Category { Id = id, Name = name, ParentId = parentId };
            List<Category> mockDB = GetCategories();
            _mockRepository.Setup(r => r.AddAsync<Category>(categoryToTest)).Callback((Category passedCategory) => {
                mockDB.Add(passedCategory);
            });
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.ParentId == parentId));
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var actualResult = await _categoryController.TryCreateCategoryAsync(categoryToTest);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync<Category>(id), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Category>(categoryToTest), Times.Once);
            Assert.True(actualResult);
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
        [Fact]
        public async Task DeleteCategoryByIdAsync_Should_Delete_Category()
        {
            //Arrange
            List<Category> mockCategoryDB = GetCategories();
            int id = 2;
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockCategoryDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.DeleteAsync<Category>(It.Is<Category>(entity => entity.Id == id)))
                .Callback((Category category) =>
                {
                    mockCategoryDB.Remove(category);
                });
            //Act
            await _categoryController.DeleteCategoryByIdAsync(id);
            //Assert
            var mustBeNull = mockCategoryDB.FirstOrDefault(x => x.Id == id);
            _mockRepository.Verify(r => r.GetByIdAsync<Category>(id), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync<Category>(It.Is<Category>(entity => entity.Id == id)), Times.Once);
            Assert.Null(mustBeNull);
        }

        [Fact]
        public async Task DeleteCategoryByIdAsync_Should_Throw_EntryNotFoundException()
        {
            //Arrange
            List<Category> mockCategoryDB = GetCategories();
            int id = 5;
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(id)).ReturnsAsync((int a) => mockCategoryDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryNotFoundException>( async ()=> await _categoryController.DeleteCategoryByIdAsync(id));
            //Assert
            _mockRepository.Verify(r => r.GetByIdAsync<Category>(id), Times.Once);
            Assert.Contains("doesn't exist", caughtException.Message);
        }
        [Fact]
        public async Task UpdateCategoryAsync_Should_Update()
        {
            //Arrange
            List<Category> mockCategoryDB = GetCategories();
            Category toUpdate = new Category { Id = 1, Name = "First set updated", ParentId = null };
            _mockRepository.Setup(r => r.GetByIdAsync<Category>(toUpdate.Id)).ReturnsAsync((int a) => mockCategoryDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.UpdateAsync<Category>(It.Is<Category>(entity => entity.Id == toUpdate.Id)))
                .Callback((Category recipe) =>
                {
                    recipe.Name = toUpdate.Name;
                });
            //Act
            await _categoryController.UpdateCategoryAsync(toUpdate);
            //Assert
            var retrieved = mockCategoryDB.FirstOrDefault(x => x.Id == toUpdate.Id);
            _mockRepository.Verify(r => r.GetByIdAsync<Category>(toUpdate.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync<Category>(It.Is<Category>(entity => entity.Id == toUpdate.Id)), Times.Once);
            Assert.Contains("updated", retrieved.Name);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Throw_EntryNotFoundException()
        {
            //Arrange
            List<Category> mockCategoryDB = GetCategories();
            Category toUpdate = new Category { Id = 5, Name = "First set updated", ParentId = null };
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryNotFoundException>( async ()=> await _categoryController.UpdateCategoryAsync(toUpdate));
            //Assert
            _mockRepository.Verify(r => r.GetByIdAsync<Category>(toUpdate.Id), Times.Once);
            Assert.Contains("doesn't exist", caughtException.Message);
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
