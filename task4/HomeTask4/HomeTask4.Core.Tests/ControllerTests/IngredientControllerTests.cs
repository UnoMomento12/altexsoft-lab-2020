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
using HomeTask4.Core.Exceptions;
namespace HomeTask4.Core.Tests.ControllerTests
{
    public class IngredientControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private IngredientController _ingredientController;
        private Mock<ILogger<IngredientController>> _loggerMock;
        private Mock<IRepository> _mockRepository;

        public IngredientControllerTests() 
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<IngredientController>>();
            _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _ingredientController = new IngredientController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact ( DisplayName = "CreateIngredientAsync method throws EmptyFieldException if name is null or empty") ]
        public async Task CreateIngredientAsync_Throws_Exception_If_Name_Null_Or_Empty()
        {
            //Arrange
            Ingredient toCreate = new Ingredient { Name =  "" };
            //Act
            var caughtException = await Assert.ThrowsAsync<EmptyFieldException>(async () => await _ingredientController.CreateIngredientAsync(toCreate));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("name is null or empty", caughtException.Message);
        }

        [Fact]
        public async Task CreateIngredientAsync_Throws_ArgumentNullException_On_Null_Reference()
        {
            //Arrange
            Ingredient ingredientToTest = null;
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _ingredientController.CreateIngredientAsync(ingredientToTest));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("reference is null", caughtException.Message);
        }

        [Fact(DisplayName = "CreateIngredientAsync method throws EntryAlreadyExistsException if ingredient already exists")]
        public async Task CreateIngredient_Throws_ArgumentException_If_Ingredient_Exists()
        {
            //Arrange
            Ingredient ingredientToTest = new Ingredient { Id = 15, Name = "carrot"};
            List<Ingredient> mockDB = GetIngredients();
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, ingredientToTest.Name, StringComparison.OrdinalIgnoreCase)));
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryAlreadyExistsException>(async () => await _ingredientController.CreateIngredientAsync(ingredientToTest));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Fact]
        public async Task CreateIngredientAsync_Creates_Ingredient()
        {
            //Arrange
            Ingredient ingredientToTest = new Ingredient { Id = 15, Name = "meat" };
            List<Ingredient> mockDB = GetIngredients();
            _mockRepository.Setup(r => r.AddAsync<Ingredient>(ingredientToTest)).Callback((Ingredient passedIngredient) => {
                mockDB.Add(passedIngredient);
            });
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(() => mockDB.FirstOrDefault(x => string.Equals(x.Name, ingredientToTest.Name, StringComparison.OrdinalIgnoreCase)));
            //Act
            await _ingredientController.CreateIngredientAsync(ingredientToTest);
            //Assert
            var retrieved = mockDB.FirstOrDefault(x => x.Name == ingredientToTest.Name);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Ingredient>(ingredientToTest), Times.Once);
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task DeleteIngredientByIdAsync_Should_Delete_Ingredient()
        {
            //Arrange
            List<Ingredient> mockIngredientDB = GetIngredients();
            int id = 2;
            _mockRepository.Setup(r => r.GetByIdAsync<Ingredient>(id)).ReturnsAsync((int a) => mockIngredientDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.DeleteAsync<Ingredient>(It.Is<Ingredient>(entity => entity.Id == id)))
                .Callback((Ingredient ingredient) =>
                {
                    mockIngredientDB.Remove(ingredient);
                });
            //Act
            await _ingredientController.DeleteIngredientByIdAsync(id);
            //Assert
            var mustBeNull = mockIngredientDB.FirstOrDefault(x => x.Id == id);
            _mockRepository.Verify(r => r.GetByIdAsync<Ingredient>(id), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync<Ingredient>(It.Is<Ingredient>(entity => entity.Id == id)), Times.Once);
            Assert.Null(mustBeNull);
        }


        [Fact]
        public async Task DeleteIngredientByIdAsync_Should_Throw_EntryNotFoundException()
        {
            //Arrange
            List<Ingredient> mockIngredientDB = GetIngredients();
            int id = 5;
            _mockRepository.Setup(r => r.GetByIdAsync<Ingredient>(id)).ReturnsAsync((int a) => mockIngredientDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryNotFoundException>( async ()=> await _ingredientController.DeleteIngredientByIdAsync(id));
            //Assert
            _mockRepository.Verify(r => r.GetByIdAsync<Ingredient>(id), Times.Once);
            Assert.Contains("doesn't exist in database", caughtException.Message);
        }
        [Fact]
        public async Task UpdateIngredientAsync_Should_Update()
        {
            //Arrange
            List<Ingredient> mockIngredientDB = GetIngredients();
            Ingredient toUpdate = new Ingredient { Id = 1, Name = "Red carrot" };
            _mockRepository.Setup(r => r.GetByIdAsync<Ingredient>(toUpdate.Id)).ReturnsAsync((int a) => mockIngredientDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.UpdateAsync<Ingredient>(It.Is<Ingredient>(entity => entity.Id == toUpdate.Id)))
                .Callback((Ingredient recipe) =>
                {
                    recipe.Name = toUpdate.Name;
                });
            //Act
            await _ingredientController.UpdateIngredientAsync(toUpdate);
            //Assert
            var retrieved = mockIngredientDB.FirstOrDefault(x => x.Id == toUpdate.Id);
            _mockRepository.Verify(r => r.GetByIdAsync<Ingredient>(toUpdate.Id), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync<Ingredient>(It.Is<Ingredient>(entity => entity.Id == toUpdate.Id)), Times.Once);
            Assert.Contains("Red", retrieved.Name);
        }

        [Fact]
        public async Task UpdateIngredientAsync_Should_Throw_EntryNotFoundException()
        {
            //Arrange
            List<Ingredient> mockIngredientDB = GetIngredients();
            Ingredient toUpdate = new Ingredient { Id = 5, Name = "Red carrot" };
            _mockRepository.Setup(r => r.GetByIdAsync<Ingredient>(toUpdate.Id)).ReturnsAsync((int a) => mockIngredientDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<EntryNotFoundException>( async ()=>  await _ingredientController.UpdateIngredientAsync(toUpdate));
            //Assert
            _mockRepository.Verify(r => r.GetByIdAsync<Ingredient>(toUpdate.Id), Times.Once);
            Assert.Contains("doesn't exist in database", caughtException.Message);
        }
        private static List<Ingredient> GetIngredients()
        {
            return new List<Ingredient>()
            {
                new Ingredient { Id = 1 , Name = "Carrot"},
                new Ingredient { Id = 2 , Name = "Sugar"},
                new Ingredient { Id = 3 , Name = "Salt"}
            };
        }
    }
}
