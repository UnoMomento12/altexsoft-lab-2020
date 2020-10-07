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
    public class CategoryControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private CategoryController _catCont;

        public CategoryControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _catCont = new CategoryController(_unitOfWork.Object, new LoggerFactory().CreateLogger<CategoryController>());
        }

        [Fact ( DisplayName = "TryCreateCategoryAsync method throws ArgumentException") ]
        public async Task TryCreateCategoryAsyncTest_Throws_Exception_On_Null_Name()
        {
            //Arange
            //Act
            var excep = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.TryCreateCategoryAsync("", 1));
            //Assert
            Assert.Contains("null or empty", excep.Message);
        }

        [Fact (DisplayName = "TryCreateCategoryAsync method return bool flags on completion")]
        public async Task TryCreateCategoryAsyncTest_Returns_Bool()
        {
            //Arange
            var cat = new Category { Id = 3, Name = "Soups", ParentId = 1 }; // new category
            var cat2 = new Category { Id = 4, Name = "First set", ParentId = null }; // overlapping category
            var repos = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            repos.Setup(r => r.ListAsync<Category>()).ReturnsAsync(mockDB);
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var result = await _catCont.TryCreateCategoryAsync(cat);
            var result2 = await _catCont.TryCreateCategoryAsync(cat2);
            //Assert
            Assert.True(result);
            Assert.False(result2);
        }

        [Fact (DisplayName = "CreateCategoryAsync method throws ArgumentNullException")]
        public async Task CreateCategoryAsyncTest_Throws_Exception()
        {
            //Arange
            Category cat1 = null;
            var repos = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            repos.Setup(r => r.ListAsync<Category>()).ReturnsAsync(mockDB);
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));

            await Assert.ThrowsAsync<ArgumentNullException>( async() => await _catCont.CreateCategoryAsync(cat1));
        }

        [Fact(DisplayName = "CreateCategoryAsync method throws ArgumentException")]
        public async Task CreateCategoryAsyncTest_Throws_ArException()
        {
            //Arange
            Category cat1 = new Category { Id = 3, Name = "First set", ParentId = null };
            Category cat2 = new Category { Id = 4, Name = "", ParentId = 100000 };
            var repos = new Mock<IRepository>();
            List<Category> mockDB = new List<Category> {
                new Category { Id = 1 , Name = "First set", ParentId = null},
                new Category { Id = 2 , Name = "Second set" , ParentId = null}
            };
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
            repos.Setup(r => r.ListAsync<Category>()).ReturnsAsync(mockDB);
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act + Assert
            var a = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat1));
            var b = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat2));
            Assert.Contains("exists", a.Message);
            Assert.Contains("empty", b.Message);
        }



    }
}
