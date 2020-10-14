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
        private Mock<IUnitOfWork> _unitOfWork;
        private RecipeController _recCont;

        public RecipeControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _recCont = new RecipeController(_unitOfWork.Object, new LoggerFactory().CreateLogger<RecipeController>());
        }

        [Fact(DisplayName = "TryCreatRecipeAsync method throws ArgumentException")]
        public async Task TryCreatRecipeAsyncTest_Throws_Exception_On_Empty_String()
        {
            //Arange
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            //Act + Assert
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            await Assert.ThrowsAsync<ArgumentException>(async () => await _recCont.TryCreateRecipeAsync("sadcaaec","",12));
            
        }

        [Fact(DisplayName = "TryCreateRecipeAsync method return bool flags on completion")]
        public async Task TryCreateRecipeAsyncTest_Returns_Bool()
        {
            //Arange
            var rec1 = new Recipe { Id = 3, Name = "Recipe 3", Description = "Description 3", CategoryId = 1 }; // new recipe
            var rec2 = new Recipe { Id = 4, Name = "Recipe 2", Description = "Description 4", CategoryId = 2 }; // overlapping recipe
            Recipe rec3 = null;
            var repos = new Mock<IRepository>();
            List<Recipe> mockDB = new List<Recipe> {
                new Recipe { Id = 1 , Name = "Recipe 1", CategoryId = 1},
                new Recipe { Id = 2 , Name = "Recipe 2" , CategoryId = 2}
            };
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>())).Callback((Recipe some) => mockDB.Add(some)); 
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(x => string.Equals(x.Name, rec1.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec1.CategoryId))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, rec1.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec1.CategoryId));
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(x => string.Equals(x.Name, rec2.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec2.CategoryId))
                 .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, rec2.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec2.CategoryId));
            repos.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var result = await _recCont.TryCreateRecipeAsync(rec1);
            var result2 = await _recCont.TryCreateRecipeAsync(rec2);
            var result3 = await _recCont.TryCreateRecipeAsync(rec3);
            //Assert
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(x => string.Equals(x.Name, rec1.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec1.CategoryId), Times.Once);
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(x => string.Equals(x.Name, rec2.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == rec2.CategoryId), Times.Once);
            Assert.True(result);
            Assert.False(result2);
            Assert.False(result3);
        }

        [Fact (DisplayName = "CreateRecipeAsync throws ArgumentNullException")]
        public async Task CreateRecipeAsyncTest_Throws_ArgumentNullException()
        {
            Recipe rec1 = null;
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            //Act + Assert
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _recCont.CreateRecipeAsync(rec1));

        }

        [Fact(DisplayName = "CreateRecipeAsync method throws ArgumentException")]
        public async Task CreateRecipeAsyncTest_Throws_ArgumentException()
        {
            //Arange
            var rec1 = new Recipe { Id = 3, Name = "", Description = "Description 3", CategoryId = 1 };
            var repos = new Mock<IRepository>();
            List<Recipe> mockDB = new List<Recipe> {
                new Recipe { Id = 1 , Name = "Recipe 1", CategoryId = 1},
                new Recipe { Id = 2 , Name = "Recipe 2" , CategoryId = 2}
            };
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddAsync<Recipe>(It.IsAny<Recipe>())).Callback((Recipe some) => mockDB.Add(some));
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            repos.Setup(r => r.GetByIdAsync<Recipe>(rec1.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act + Assert
            var aEx = await Assert.ThrowsAsync<ArgumentException>(async () => await _recCont.CreateRecipeAsync(rec1));
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            repos.Verify(repos => repos.GetByIdAsync<Recipe>(rec1.Id), Times.Never);
            Assert.Contains("Name is empty", aEx.Message);
        }

        [Fact(DisplayName = "AddIngredientToRecipeAsync method throws ArgumentException on non-existent recipe")]
        public async Task AddIngredientToRecipeAsync_Throws_ArgumentException_On_NonExisting_Recipe()
        {
            //Arrange
            #region SameIngredientDetailArrange
            List<Recipe> mockRecipeDB = GetRecipes();
            List<Measure> mockMeasureDB = GetMeasures();
            List<Ingredient> mockIngredientDB = GetIngredients();
            List<IngredientDetail> mockIDDB = GetIngredientDetails();
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            repos.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                .Callback((Measure some) => {
                    some.Id = mockMeasureDB.Last().Id + 1;
                    mockMeasureDB.Add(some);
                });
            repos.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) => {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            repos.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) => {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            repos.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount+=some.Amount;
                });
            #endregion
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>())).ReturnsAsync((Recipe)null);
            repos.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()));
            repos.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()));
            repos.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()));

            Recipe targerRecipe1 = new Recipe {Id = 0, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "beef";
            double amount = 800;
            string measure = "g";
            //Act + Assert
            var argEx1 = await Assert.ThrowsAsync<ArgumentException>(async () =>await  _recCont.AddIngredientToRecipeAsync(targerRecipe1, ingName, measure, amount));
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()),Times.Once);
            repos.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<Measure>(It.IsAny<Measure>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Never);
            Assert.Contains("doesn't exist in Database", argEx1.Message);
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
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            repos.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                 .Callback((Measure some) => {
                     some.Id = mockMeasureDB.Last().Id + 1;
                     mockMeasureDB.Add(some);
                 });
            repos.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) => {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            repos.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) => {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            repos.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount += some.Amount;
                });
            #endregion
            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()));
            repos.Setup(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()));
            repos.Setup(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()));
            repos.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()));
            Recipe targerRecipe1 = new Recipe { Id = 1 , Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "";
            double amount = 800;
            string measure = "g";
            //Act + Assert
            var argEx1 = await Assert.ThrowsAsync<ArgumentException>(async () => await _recCont.AddIngredientToRecipeAsync(targerRecipe1, ingName, measure, amount));
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<Measure>(It.IsAny<Measure>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Never);
            Assert.Contains("IngredientName is empty", argEx1.Message);
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
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            //----------------------------------------------------------------------------------------------------------------------------------------//
            repos.Setup(r => r.AddAsync<Measure>(It.IsAny<Measure>()))
                .Callback((Measure some) => { 
                    some.Id = mockMeasureDB.Last().Id + 1;
                    mockMeasureDB.Add(some); 
                });
            repos.Setup(r => r.AddAsync<Ingredient>(It.IsAny<Ingredient>()))
                .Callback((Ingredient some) => {
                    some.Id = mockIngredientDB.Last().Id + 1;
                    mockIngredientDB.Add(some);
                });

            repos.Setup(r => r.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) => {
                    some.Id = mockIDDB.Last().Id + 1;
                    mockIDDB.Add(some);
                });
            repos.Setup(r => r.UpdateAsync<IngredientDetail>(It.IsAny<IngredientDetail>()))
                .Callback((IngredientDetail some) =>
                {
                    mockIDDB.SingleOrDefault(x => x.RecipeId == some.RecipeId && x.IngredientId == x.IngredientId).Amount += some.Amount;
                });
            #endregion
            Recipe targetRecipe1 = new Recipe { Id = 1, Name = "Recipe 1", CategoryId = 1 }; // Non-existent in DB recipe
            string ingName = "Salmon"; // id = 6
            double amount = 6;
            string measure = "fille"; //id = 5
            IngredientDetail detail = new IngredientDetail { RecipeId = targetRecipe1.Id, IngredientId = 6, MeasureId = 5 };

            repos.Setup(r => r.FirstOrDefaultAsync<Recipe>(x => string.Equals(x.Name, targetRecipe1.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == targetRecipe1.CategoryId && x.Id == targetRecipe1.Id))
                .ReturnsAsync(() => mockRecipeDB.SingleOrDefault(x => string.Equals(x.Name, targetRecipe1.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == targetRecipe1.CategoryId && x.Id == targetRecipe1.Id));
            repos.Setup(r => r.FirstOrDefaultAsync<Ingredient>(x => string.Equals(x.Name, ingName, StringComparison.OrdinalIgnoreCase)))
                .ReturnsAsync(() => mockIngredientDB.SingleOrDefault(x => string.Equals(x.Name, ingName, StringComparison.OrdinalIgnoreCase)));
            repos.Setup(r => r.FirstOrDefaultAsync<Measure>(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase)))
                .ReturnsAsync(() => mockMeasureDB.SingleOrDefault(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase)));
            repos.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(x => x.RecipeId == detail.RecipeId && x.IngredientId == detail.IngredientId))
                .ReturnsAsync(() => mockIDDB.SingleOrDefault(x => x.RecipeId == detail.RecipeId && x.IngredientId == detail.IngredientId));
            //Act
            await _recCont.AddIngredientToRecipeAsync(targetRecipe1, ingName, measure, amount);
            IngredientDetail retrieved = mockIDDB.SingleOrDefault(x => x.RecipeId == targetRecipe1.Id && x.IngredientId == mockIngredientDB.SingleOrDefault(x => x.Name == ingName)?.Id);
            //Assert
            repos.Verify(r => r.FirstOrDefaultAsync<Recipe>(It.IsAny<Expression<Func<Recipe, bool>>>()), Times.Once);
            repos.Verify(r => r.FirstOrDefaultAsync<Measure>(It.IsAny<Expression<Func<Measure, bool>>>()), Times.Once);
            repos.Verify(r => r.FirstOrDefaultAsync<Ingredient>(It.IsAny<Expression<Func<Ingredient, bool>>>()), Times.Once);
            repos.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Once);
            repos.Verify(repos => repos.AddAsync<Measure>(It.IsAny<Measure>()), Times.Once);
            repos.Verify(repos => repos.AddAsync<Ingredient>(It.IsAny<Ingredient>()), Times.Once);
            repos.Verify(repos => repos.AddAsync<IngredientDetail>(It.IsAny<IngredientDetail>()), Times.Once);
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