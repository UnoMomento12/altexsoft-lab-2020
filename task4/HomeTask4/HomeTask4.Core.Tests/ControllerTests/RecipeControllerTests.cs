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
            Recipe testedRecipe = new Recipe { Name = "Some Recipe", Description = "", CategoryId = 12 };
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _mockRepository.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            _mockRepository.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(testedRecipe));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()), Times.Never);
            _mockRepository.Verify(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()), Times.Never);
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
            Recipe testedRecipe = new Recipe { Name = "", Description = "Description", CategoryId = 12 };
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object); 
            _mockRepository.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            _mockRepository.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(testedRecipe));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()), Times.Never);
            _mockRepository.Verify(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()), Times.Never);
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
        public async Task TryCreateRecipeAsync_Returns_Bool(int id, string name, string description, int? categoryId , bool isProblem, bool result)
        {
            //Arrange
            var testedRecipe = new Recipe { Id = id, Name = name, Description = description, CategoryId = categoryId }; // new recipe
            var _mockRepository = new Mock<IRepository>();
            List<Recipe> mockDB = GetRecipes();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            if (!isProblem)
            {
                _mockRepository.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>())).Callback((Recipe some) => mockDB.Add(some));
            }
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, testedRecipe.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == testedRecipe.CategoryId));
            _mockRepository.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var resultFlag = await _recipeController.TryCreateRecipeAsync(testedRecipe);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()), Times.Once);
            _mockRepository.Verify(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()), Times.Once);
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
            Assert.True(resultFlag == result);
        }
        [Fact(DisplayName = "TryCreateRecipeAsync method throws argument exception on duplicate recipe")]
        public async Task TryCreateRecipeAsync_Throws_ArgumentException_On_Recipe_Duplicate()
        {
            //Arrange
            var testedRecipe = new Recipe { Id = 4, Name = "Recipe 2", Description = "Description 4", CategoryId = 2 }; 
            var _mockRepository = new Mock<IRepository>();
            List<Recipe> mockDB = GetRecipes();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _mockRepository.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>())).Callback((Recipe some) => mockDB.Add(some));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, testedRecipe.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == testedRecipe.CategoryId));
            _mockRepository.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.TryCreateRecipeAsync(testedRecipe));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync<Recipe>(It.IsAny<Recipe>()), Times.Never);
            _mockRepository.Verify(r => r.GetByIdAsync<Recipe>(It.IsAny<int>()), Times.Never);
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
            Recipe testedRecipe = null;
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            //Act
            var resultFlag = await _recipeController.TryCreateRecipeAsync(testedRecipe);
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            Assert.False(resultFlag);
        }


        [Fact(DisplayName = "AddIngredientToRecipeAsync method throws ArgumentException on non-existent recipe")]
        public async Task AddIngredientToRecipeAsync_Throws_ArgumentException_On_NonExistent_Recipe()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<Measure> mockMeasureDB = GetMeasures();
            List<Ingredient> mockIngredientDB = GetIngredients();
            List<IngredientDetail> mockIDDB = GetIngredientDetails();
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            _mockRepository.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                .Callback((Measure some) =>
                {
                    some.Id = mockMeasureDB.Last().Id + 1;
                    mockMeasureDB.Add(some);
                });
            _mockRepository.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) =>
                {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            _mockRepository.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            _mockRepository.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount += some.Amount;
                });
            #endregion
            Recipe testedRecipe = new Recipe { Id = 0, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "beef";
            double amount = 800;
            string measure = "g";
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.FirstOrDefault(x => string.Equals(x.Name, testedRecipe.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == testedRecipe.CategoryId && x.Id == testedRecipe.Id));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.AddIngredientToRecipeAsync(testedRecipe, ingName, measure, amount));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Measure>(It.IsAny<Measure>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Never);
            Assert.Contains("doesn't exist in Database", caughtException.Message);
        }

        [Fact(DisplayName = "AddIngredientToRecipeAsync method throws ArgumentException on empty ingredient name")]
        public async Task AddIngredientToRecipeAsync_Throws_ArgumentException_On_Empty_IngredientName()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<Measure> mockMeasureDB = GetMeasures();
            List<Ingredient> mockIngredientDB = GetIngredients();
            List<IngredientDetail> mockIDDB = GetIngredientDetails();
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            _mockRepository.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                 .Callback((Measure some) =>
                 {
                     some.Id = mockMeasureDB.Last().Id + 1;
                     mockMeasureDB.Add(some);
                 });
            _mockRepository.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) =>
                {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            _mockRepository.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            _mockRepository.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount += some.Amount;
                });
            #endregion
            Recipe testedRecipe = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "";
            double amount = 800;
            string measure = "g";
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()));
            //Act
            var caughtException = await Assert.ThrowsAsync<ArgumentException>(async () => await _recipeController.AddIngredientToRecipeAsync(testedRecipe, ingName, measure, amount));
            //Assert
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Measure>(It.IsAny<Measure>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Never);
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
            var _mockRepository = new Mock<IRepository>();
            _unitOfWorkMock.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            _mockRepository.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                .Callback((Measure some) =>
                {
                    some.Id = mockMeasureDB.Last().Id + 1;
                    mockMeasureDB.Add(some);
                });
            _mockRepository.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) =>
                {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            _mockRepository.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            _mockRepository.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount += some.Amount;
                });
            #endregion
            Recipe testedRecipe = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "Salmon"; // id = 6
            double amount = 6;
            string measure = "fille"; //id = 5
            IngredientDetail detail = new IngredientDetail { RecipeId = testedRecipe.Id, IngredientId = 6, MeasureId = 5 };

            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(() => mockRecipeDB.SingleOrDefault(x => string.Equals(x.Name, testedRecipe.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == testedRecipe.CategoryId && x.Id == testedRecipe.Id));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()))
                .ReturnsAsync(() => mockIngredientDB.SingleOrDefault(x => string.Equals(x.Name, ingName, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()))
                .ReturnsAsync(() => mockMeasureDB.SingleOrDefault(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase)));
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()))
                .ReturnsAsync(() => mockIDDB.SingleOrDefault(x => x.RecipeId == detail.RecipeId && x.IngredientId == detail.IngredientId));
            //Act
            await _recipeController.AddIngredientToRecipeAsync(testedRecipe, ingName, measure, amount);
            IngredientDetail retrieved = mockIDDB.SingleOrDefault(x => x.RecipeId == testedRecipe.Id && x.IngredientId == mockIngredientDB.SingleOrDefault(x => x.Name == ingName)?.Id);
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