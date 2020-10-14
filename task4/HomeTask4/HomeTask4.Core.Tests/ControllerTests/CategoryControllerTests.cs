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
        public async Task TryCreateCategoryAsyncTest_Throws_Exception_On_Empty_Name()
        {
            //Arange
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>()));
            repos.Setup(r => r.AddAsync<Category>(It.IsAny<Category>()));
            //Act
            var excep = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.TryCreateCategoryAsync("", 1));
            //Assert
            repos.Verify(repos => repos.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
            repos.Verify(repos => repos.AddAsync<Category>(null), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
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
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(x => string.Equals(x.Name, cat.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat.ParentId))
                .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat.ParentId));
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId))
                .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId));
            repos.Setup(r => r.GetByIdAsync<Category>(cat.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            repos.Setup(r => r.GetByIdAsync<Category>(cat2.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var result = await _catCont.TryCreateCategoryAsync(cat);
            var result2 = await _catCont.TryCreateCategoryAsync(cat2);

            //Assert
            repos.Verify(repos => repos.AddAsync<Category>(cat), Times.Once);
            repos.Verify(repos => repos.AddAsync<Category>(cat2), Times.Never);
            repos.Verify(repos => repos.GetByIdAsync<Category>(cat.Id), Times.Once);
            repos.Verify(repos => repos.GetByIdAsync<Category>(cat2.Id), Times.Once);
            repos.Verify(repos => repos.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Exactly(2));
            Assert.True(result);
            Assert.False(result2);
        }

        [Fact (DisplayName = "CreateCategoryAsync method throws ArgumentNullException")]
        public async Task CreateCategoryAsyncTest_Throws_ArgumentNullException()
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
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            //Act
            var ex = await Assert.ThrowsAsync<ArgumentNullException>( async() => await _catCont.CreateCategoryAsync(cat1));
            //Assert
            repos.Verify(r => r.AddAsync<Category>(cat1), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
            repos.Verify(repos => repos.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
            Assert.Contains("reference is null", ex.Message);
        }

        [Fact(DisplayName = "CreateCategoryAsync method throws ArgumentException")]
        public async Task CreateCategoryAsyncTest_Throws_ArgumentException()
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
            repos.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(x => string.Equals(x.Name, cat1.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat1.ParentId))
                .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat1.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat1.ParentId));
            repos.Setup(r => r.FirstOrDefaultAsync<Category>(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId))
                .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId));
            //Act + Assert
            var aEx = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat1));
            var bEx = await Assert.ThrowsAsync<ArgumentException>(async () => await _catCont.CreateCategoryAsync(cat2));
            repos.Verify(repos => repos.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
            repos.Verify(r => r.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Exactly(1));
            Assert.Contains("exists", aEx.Message);
            Assert.Contains("empty", bEx.Message);
        }
    }
}
