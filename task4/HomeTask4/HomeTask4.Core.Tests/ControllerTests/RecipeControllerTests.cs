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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;
namespace HomeTask4.Core.Tests.ControllerTests
{
    public class RecipeControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private RecipeController _recipeController;
        private Mock<ILogger<RecipeController>> _loggerMock;
        private Mock<IRepository> _mockRepository;

        public RecipeControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RecipeController>>();
            _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _recipeController = new RecipeController(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact(DisplayName = "TryCreatRecipeAsync method throws ArgumentException on empty description")]
        public async Task TryCreatRecipeAsync_Throws_ArgumentException_On_Empty_Description()
        {
            //Arrange
            Recipe recipeToTest = new Recipe { Name = "Some Recipe", Description = "", CategoryId = 12 };
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(recipeToTest));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("Recipe description is null or empty", caughtException.Message);
        }

        [Fact(DisplayName = "TryCreatRecipeAsync method throws ArgumentException on empty name")]
        public async Task TryCreatRecipeAsync_Throws_ArgumentException_On_Empty_Name()
        {
            //Arrange
            Recipe recipeToTest = new Recipe { Name = "", Description = "Description", CategoryId = 12 };
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(recipeToTest));
            //Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("Recipe name is null or empty", caughtException.Message);
        }

        [Theory(DisplayName = "TryCreateRecipeAsync method return bool flag on completion")]
        [InlineData(3, "Recipe 3", "Description 3", 1, false, true)]
        [InlineData(3, "Recipe 3", "Description 3", 1, true, false)]
        public async Task TryCreateRecipeAsync_Returns_Bool(int id, string name, string description, int? categoryId, bool isProblem, bool result)
        {
            //Arrange
            var recipeToTest = new Recipe { Id = id, Name = name, Description = description, CategoryId = categoryId }; // new recipe
            List<Recipe> mockDB = GetRecipes();
            if (!isProblem)
            {
                _mockRepository.Setup(r => r.AddAsync<Recipe>(recipeToTest)).Callback((Recipe passedRecipe) => {
                    mockDB.Add(passedRecipe);
                });
            }
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId));
            _mockRepository.Setup(r => r.GetByIdAsync<Recipe>(id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var resultFlag = await _recipeController.TryCreateRecipeAsync(recipeToTest);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Recipe>(recipeToTest), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync<Recipe>(id), Times.Once);
            Assert.True(resultFlag == result);
        }
        [Fact(DisplayName = "TryCreateRecipeAsync method throws argument exception on duplicate recipe")]
        public async Task TryCreateRecipeAsync_Throws_ArgumentException_On_Recipe_Duplicate()
        {
            //Arrange
            var recipeToTest = new Recipe { Id = 4, Name = "Recipe 2", Description = "Description 4", CategoryId = 2 };
            List<Recipe> mockDB = GetRecipes();
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(recipeToTest));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
            Assert.Contains("already exists", caughtException.Message);
        }

        [Fact(DisplayName = "TryCreateRecipeAsync doesn't reach exception throws and returns false on null reference")]
        public async Task TryCreateRecipeAsync_Doesnt_Reach_Exception_Throws_And_Returns_False()
        {
            Recipe recipeToTest = null;
            //Act
            var resultFlag = await _recipeController.TryCreateRecipeAsync(recipeToTest);
            //Assert
            Assert.False(resultFlag);
        }


        [Fact(DisplayName = "AddIngredientToRecipeAsync method throws ArgumentException on non-existent recipe")]
        public async Task AddIngredientToRecipeAsync_Throws_ArgumentException_On_NonExistent_Recipe()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            #endregion
            Recipe recipeToTest = new Recipe { Id = 0, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingredientName = "beef";
            double amount = 800;
            string measure = "g";
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.FirstOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId && x.Id == recipeToTest.Id));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.AddIngredientToRecipeAsync(recipeToTest, ingredientName, measure, amount));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            Assert.Contains("doesn't exist in Database", caughtException.Message);
        }

        [Fact(DisplayName = "AddIngredientToRecipeAsync method throws ArgumentException on empty ingredient name")]
        public async Task AddIngredientToRecipeAsync_Throws_ArgumentException_On_Empty_IngredientName()
        {
            Recipe recipeToTest = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 };
            string ingredientName = "";
            double amount = 800;
            string measure = "g";
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.AddIngredientToRecipeAsync(recipeToTest, ingredientName, measure, amount));
            //Assert
            Assert.Contains("IngredientName is empty", caughtException.Message);
        }

        [Fact(DisplayName = "AddIngredientToRecipeAsync method creates new entry in tables of database on each step")]
        public async Task AddIngredientToRecipeAsync_Creates_New_DataBase_Entry_On_Each_Step()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<Measure> mockMeasureDB = GetMeasures();
            List<Ingredient> mockIngredientDB = GetIngredients();
            List<IngredientDetail> mockIDDB = GetIngredientDetails();
            //----------------------------------------------------------------------------------------------------------------------------------------//
            _mockRepository.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                .Callback((Measure measure) =>
                {
                    measure.Id = mockMeasureDB.Last().Id + 1;
                    mockMeasureDB.Add(measure);
                });
            _mockRepository.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient ingredient) =>
                {
                    ingredient.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(ingredient);
                });

            _mockRepository.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail ingredientDetail) =>
                {
                    ingredientDetail.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(ingredientDetail);
                });
            #endregion
            Recipe recipeToTest = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 };
            string ingredientName = "Salmon"; // id = 6
            double amount = 6;
            string measure = "fille"; //id = 5
            IngredientDetail detail = new IngredientDetail { RecipeId = recipeToTest.Id, IngredientId = 6, MeasureId = 5 };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.SingleOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId && x.Id == recipeToTest.Id));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(() => mockIngredientDB.SingleOrDefault(x => string.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()))
                .ReturnsAsync(() => mockMeasureDB.SingleOrDefault(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()))
                .ReturnsAsync(() => mockIDDB.SingleOrDefault(x => x.RecipeId == detail.RecipeId && x.IngredientId == detail.IngredientId));
            //Act
            await _recipeController.AddIngredientToRecipeAsync(recipeToTest, ingredientName, measure, amount);
            IngredientDetail retrieved = mockIDDB.SingleOrDefault(x => x.RecipeId == recipeToTest.Id && x.IngredientId == mockIngredientDB.SingleOrDefault(x => x.Name == ingredientName)?.Id);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Once);
            _mockRepository.Verify(_mockRepository => _mockRepository.AddAsync<Measure>(It.IsAny<Measure>()), Times.Once);
            _mockRepository.Verify(_mockRepository => _mockRepository.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Once);
            _mockRepository.Verify(_mockRepository => _mockRepository.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Once);
            Assert.NotNull(retrieved);
        }

        [Fact(DisplayName = "AddIngredientToRecipeAsync method updates amount value of ingredient detail")]
        public async Task AddIngredientToRecipeAsync_Updates_IngredientDetail_Amount()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<Measure> mockMeasureDB = GetMeasures();
            List<Ingredient> mockIngredientDB = GetIngredients();
            List<IngredientDetail> mockIDDB = GetIngredientDetails();
            _mockRepository.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail ingredientDetail) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == ingredientDetail.RecipeId && x.IngredientId == ingredientDetail.IngredientId).Amount += ingredientDetail.Amount;
                });
            #endregion
            Recipe recipeToTest = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 };
            string ingredientName = "Sugar"; 
            double amount = 200;
            string measure = "g"; 
            IngredientDetail detail = new IngredientDetail { RecipeId = recipeToTest.Id, IngredientId = 2, MeasureId = 3 };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.SingleOrDefault(x => string.Equals(x.Name, recipeToTest.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipeToTest.CategoryId && x.Id == recipeToTest.Id));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(() => mockIngredientDB.SingleOrDefault(x => string.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()))
                .ReturnsAsync(() => mockMeasureDB.SingleOrDefault(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()))
                .ReturnsAsync(() => mockIDDB.SingleOrDefault(x => x.RecipeId == detail.RecipeId && x.IngredientId == detail.IngredientId));
            //Act
            await _recipeController.AddIngredientToRecipeAsync(recipeToTest, ingredientName, measure, amount);
            IngredientDetail retrieved = mockIDDB.SingleOrDefault(x => x.RecipeId == recipeToTest.Id && x.IngredientId == mockIngredientDB.SingleOrDefault(x => x.Name == ingredientName)?.Id);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Once);
            _mockRepository.Verify(_mockRepository => _mockRepository.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Once);
            Assert.NotNull(retrieved);
        }
        private List<Recipe> GetRecipes()
        {
            return new List<Recipe> {
                new Recipe { Id = 1 , Name = "Recipe 1", CategoryId = 1},
                new Recipe { Id = 2 , Name = "Recipe 2" , CategoryId = 2}
            };
        }
        private List<Measure> GetMeasures()
        {
            return new List<Measure> {
                new Measure{ Id = 1 , Name = "ml"},
                new Measure{ Id = 2 , Name = "kg" },
                new Measure{ Id = 3 , Name = "g" },
                new Measure{ Id = 4 , Name = "l" }
            };
        }
        private List<Ingredient> GetIngredients()
        {
            return new List<Ingredient> {
                new Ingredient{ Id = 1 , Name = "Carrot"},
                new Ingredient{ Id = 2 , Name = "Sugar" },
                new Ingredient{ Id = 3 , Name = "Water" },
                new Ingredient{ Id = 4 , Name = "Milk" },
                new Ingredient{ Id = 5 , Name = "Chocolate" }
            };
        }
        private List<IngredientDetail> GetIngredientDetails()
        {
            return new List<IngredientDetail>
            {
                new IngredientDetail { RecipeId = 1 , IngredientId = 2 , Amount = 80, MeasureId = 3},
                new IngredientDetail { RecipeId = 1 , IngredientId = 3 , Amount = 2, MeasureId = 4}
            };
        }

    }
}