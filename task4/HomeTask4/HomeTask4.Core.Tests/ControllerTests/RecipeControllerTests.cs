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
            //Act + Assert
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
            repos.Setup(r => r.ListAsync<Recipe>()).ReturnsAsync(mockDB);
            repos.Setup(r => r.GetByIdAsync<Recipe>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var result = await _recCont.TryCreateRecipeAsync(rec1);
            var result2 = await _recCont.TryCreateRecipeAsync(rec2);
            var result3 = await _recCont.TryCreateRecipeAsync(rec3);
            //Assert
            Assert.True(result);
            Assert.False(result2);
            Assert.False(result3);
        }

        [Fact (DisplayName = "CreateRecipeAsync throws ArgumentNullException")]
        public async Task CreateRecipeAsyncTest_Throws_ArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _recCont.CreateRecipeAsync(null));
        }

        //[Fact(DisplayName = "CreateCategoryAsync method throws ArgumentNullException")]
        //public async Task CreateCategoryAsyncTest_Throws_Exception()
        //{
        //    //Arange
        //    Category cat1 = null;
        //    var repos = new Mock<IRepository>();
        //    List<Category> mockDB = new List<Category> {
        //        new Category { Id = 1 , Name = "First set", ParentId = null},
        //        new Category { Id = 2 , Name = "Second set" , ParentId = null}
        //    };
        //    _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
        //    repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
        //    repos.Setup(r => r.ListAsync<Category>()).ReturnsAsync(mockDB);
        //    repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));

        //    await Assert.ThrowsAsync<ArgumentNullException>(async () => await _catCont.CreateCategoryAsync(cat1));
        //}

        //[Fact(DisplayName = "CreateCategoryAsync method throws ArgumentException")]
        //public async Task CreateCategoryAsyncTest_Throws_ArException()
        //{
        //    //Arange
        //    Category cat1 = new Category { Id = 3, Name = "First set", ParentId = null };
        //    Category cat2 = new Category { Id = 4, Name = "", ParentId = 100000 };
        //    var repos = new Mock<IRepository>();
        //    List<Category> mockDB = new List<Category> {
        //        new Category { Id = 1 , Name = "First set", ParentId = null},
        //        new Category { Id = 2 , Name = "Second set" , ParentId = null}
        //    };
        //    _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
        //    repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
        //    repos.Setup(r => r.ListAsync<Category>()).ReturnsAsync(mockDB);
        //    repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
        //    //Act + Assert
        //    var a = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat1));
        //    var b = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat2));
        //    Assert.Contains("exists", a.Message);
        //    Assert.Contains("empty", b.Message);
        //}



    }
}