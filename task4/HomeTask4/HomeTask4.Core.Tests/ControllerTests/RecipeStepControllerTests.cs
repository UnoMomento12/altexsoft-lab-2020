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
    public class RecipeStepControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private RecipeStepController _recipeStepController;
        private Mock<ILogger<RecipeStepController>> _loggerMock;
        private Mock<IRepository> _mockRepository;

        public RecipeStepControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRepository>();
            _loggerMock = new Mock<ILogger<RecipeStepController>>();
            _unitOfWork.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _recipeStepController = new RecipeStepController(_unitOfWork.Object, _loggerMock.Object);
        }
        [Fact(DisplayName = "AddStepToRecipeAsync method throws ArgumentException on recipe null reference")]
        public async Task AddStepToRecipeAsync_Throws_ArgumentException_On_Recipe_Null_Reference()
        {
            //Arrange
            Recipe recipeToTest = null;
            string step = "test step";
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeStepController.AddStepToRecipeAsync(recipeToTest, step));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("Recipe reference is null", caughtException.Message);
        }
        [Fact(DisplayName = "AddStepToRecipeAsync method throws ArgumentException on empty step description")]
        public async Task AddStepToRecipeAsync_Throws_ArgumentException_On_Empty_Description()
        {
            //Arrange
            Recipe recipeToTest = new Recipe { Name = "Some Recipe", Description = "some desc", CategoryId = 12 };
            string step = "";
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeStepController.AddStepToRecipeAsync(recipeToTest, step));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("Description is empty", caughtException.Message);
        }

        [Fact(DisplayName = "AddStepToRecipeAsync method throws ArgumentException on non-existent recipe")]
        public async Task AddStepToRecipeAsync_Throws_ArgumentException_On_NonExistent_Recipe()
        {
            //Arrange
            List<Recipe> mockRecipeDB = GetRecipes();
            Recipe recipeToTest = new Recipe { Id = 15, Name = "Some Recipe", Description = "some desc", CategoryId = 12 };
            string step = "test step";
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                 .ReturnsAsync(() => mockRecipeDB.FirstOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeStepController.AddStepToRecipeAsync(recipeToTest, step));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("doesn't exist in Database", caughtException.Message);
        }
        [Fact(DisplayName = "AddStepsAsync method adds RecipeStep to DB ")]
        public async Task AddStepsAsync()
        {
            //Arrange
            List<RecipeStep> recipeStepsDB = new List<RecipeStep>();
            List<RecipeStep> stepsToAdd = GetSteps();
            _mockRepository.Setup(r => r.AddRangeAsync<RecipeStep>(stepsToAdd))
                 .Callback((List<RecipeStep> passedList) =>
                 {
                     recipeStepsDB.AddRange(passedList);
                 });
            //Act
            await _recipeStepController.AddStepsAsync(stepsToAdd);
            //Assert
            _mockRepository.Verify(r => r.AddRangeAsync<RecipeStep>(stepsToAdd), Times.Once);
            Assert.Equal(recipeStepsDB, stepsToAdd);
        }
        [Fact]
        public async Task AddStepToRecipeAsync_Should_Add_Step()
        {
            //Arrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<RecipeStep> mockStepsDB = GetSteps();
            Recipe target = new Recipe { Id = 1, Name = "Recipe 1", Description = "desc1", CategoryId = 1, Steps = new List<RecipeStep>() };
            string newStep = "test step"; //stepnnumber = 1
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.SingleOrDefault(x => string.Equals(x.Name, target.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == target.CategoryId && x.Id == target.Id));
            _mockRepository.Setup(r => r.UpdateAsync<Recipe>(It.Is<Recipe>(x=>x.Name == target.Name && x.Id == target.Id)))
                .Callback((Recipe recipe) =>
                {
                    RecipeStep toAdd = new RecipeStep { Id = 1, Description = newStep, RecipeId = recipe.Id, StepNumber = recipe.Steps.Count + 1 };
                    mockStepsDB.Add(toAdd);
                });
            //Act
            await _recipeStepController.AddStepToRecipeAsync(target, newStep);
            //Assert
            var retrieved = mockStepsDB.FirstOrDefault(x => x.Description == newStep);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync<Recipe>(It.Is<Recipe>(x => x.Name == target.Name && x.Id == target.Id)), Times.Once);
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task DeleteStepByIdAsync_Should_Delete_Recipe()
        {
            //Arrange
            List<RecipeStep> mockRecipeStepDB = GetSteps();
            int id = 2;
            _mockRepository.Setup(r => r.GetByIdAsync<RecipeStep>(id)).ReturnsAsync((int a) => mockRecipeStepDB.FirstOrDefault(x => x.Id == a));
            _mockRepository.Setup(r => r.DeleteAsync<RecipeStep>(It.Is<RecipeStep>(entity => entity.Id == id)))
                .Callback((RecipeStep recipeStep) =>
                {
                    mockRecipeStepDB.Remove(recipeStep);
                });
            //Act
            await _recipeStepController.DeleteStepByIdAsync(id);
            //Assert
            var mustBeNull = mockRecipeStepDB.FirstOrDefault(x => x.Id == id);
            _mockRepository.Verify(r => r.GetByIdAsync<RecipeStep>(id), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync<RecipeStep>(It.Is<RecipeStep>(entity => entity.Id == id)), Times.Once);
            Assert.Null(mustBeNull);
        }
        private List<Recipe> GetRecipes()
        {
            return new List<Recipe> {
                new Recipe { Id = 1 , Name = "Recipe 1", CategoryId = 1, Ingredients = new List<IngredientDetail>(), Steps = new List<RecipeStep>()},
                new Recipe { Id = 2 , Name = "Recipe 2", CategoryId = 2, Ingredients = new List<IngredientDetail>(), Steps = new List<RecipeStep>()}
            };
        }
        private List<RecipeStep> GetSteps()
        {
            return new List<RecipeStep>
            {
                new RecipeStep{ Id = 1 , Description = "step 1.1" , RecipeId = 1, StepNumber = 1},
                new RecipeStep{ Id = 2 , Description = "step 1.2" , RecipeId = 1, StepNumber = 2},
                new RecipeStep{ Id = 3 , Description = "step 1.3" , RecipeId = 1, StepNumber = 3},
                new RecipeStep{ Id = 4 , Description = "step 2.1" , RecipeId = 2, StepNumber = 1},
                new RecipeStep{ Id = 5 , Description = "step 2.2" , RecipeId = 2, StepNumber = 2},
                new RecipeStep{ Id = 6 , Description = "step 2.3" , RecipeId = 2, StepNumber = 3},
            };
        }
    }
}
