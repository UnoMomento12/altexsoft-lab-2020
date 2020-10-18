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
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<MockCategoryController> _categoryController;

        public CategoryControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _categoryController = new Mock<MockCategoryController>(_unitOfWork.Object, new LoggerFactory().CreateLogger<CategoryController>());
            _categoryController.SetupGet(c => c.UnitOfWork).Returns(_unitOfWork.Object);
        }

        [Fact ( DisplayName = "TryCreateCategoryAsync method throws ArgumentException") ]
        public async Task TryCreateCategoryAsyncTest_Throws_Exception_On_Empty_Name()
        {
            //Arange
            string exceptionMessage;
            var mockRepository = new Mock<IRepository>();
            _unitOfWork.SetupGet(u => u.Repository).Returns(mockRepository.Object);
            _categoryController.Setup(c => c.Logger.LogInformation(It.IsAny<string>())).Callback<string>((message) => exceptionMessage = message);
            mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
            mockRepository.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>()));
            mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>()));
            
            //Act
            await _categoryController.Object.TryCreateCategoryAsync("", 1);
            //Assert
            mockRepository.Verify(mockRepository => mockRepository.GetByIdAsync<Category>(It.IsAny<int>()), Times.Once);
            mockRepository.Verify(mockRepository => mockRepository.AddAsync<Category>(null), Times.Never);
            mockRepository.Verify(r => r.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
            Assert.Contains("null or empty", excep.Message);
        }

        //[Fact (DisplayName = "TryCreateCategoryAsync method return bool flags on completion")]
        //public async Task TryCreateCategoryAsyncTest_Returns_Bool()
        //{
        //    //Arange
        //    var cat = new Category { Id = 3, Name = "Soups", ParentId = 1 }; // new category
        //    var cat2 = new Category { Id = 4, Name = "First set", ParentId = null }; // overlapping category
        //    var mockRepository = new Mock<IRepository>();
        //    List<Category> mockDB = new List<Category> {
        //        new Category { Id = 1 , Name = "First set", ParentId = null},
        //        new Category { Id = 2 , Name = "Second set" , ParentId = null}
        //    };
        //    _unitOfWork.Setup(u => u.Repository).Returns(mockRepository.Object);
        //    mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
        //    mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(x => string.Equals(x.Name, cat.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat.ParentId))
        //        .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat.ParentId));
        //    mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId))
        //        .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId));
        //    mockRepository.Setup(r => r.GetByIdAsync<Category>(cat.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
        //    mockRepository.Setup(r => r.GetByIdAsync<Category>(cat2.Id)).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
        //    //Act
        //    var result = await _categoryController.TryCreateCategoryAsync(cat);
        //    var result2 = await _categoryController.TryCreateCategoryAsync(cat2);

        //    //Assert
        //    mockRepository.Verify(mockRepository => mockRepository.AddAsync<Category>(cat), Times.Once);
        //    mockRepository.Verify(mockRepository => mockRepository.AddAsync<Category>(cat2), Times.Never);
        //    mockRepository.Verify(mockRepository => mockRepository.GetByIdAsync<Category>(cat.Id), Times.Once);
        //    mockRepository.Verify(mockRepository => mockRepository.GetByIdAsync<Category>(cat2.Id), Times.Once);
        //    mockRepository.Verify(mockRepository => mockRepository.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Exactly(2));
        //    Assert.True(result);
        //    Assert.False(result2);
        //}

        //[Fact (DisplayName = "CreateCategoryAsync method throws ArgumentNullException")]
        //public async Task CreateCategoryAsyncTest_Throws_ArgumentNullException()
        //{
        //    //Arange
        //    Category cat1 = null;
        //    var mockRepository = new Mock<IRepository>();
        //    List<Category> mockDB = new List<Category> {
        //        new Category { Id = 1 , Name = "First set", ParentId = null},
        //        new Category { Id = 2 , Name = "Second set" , ParentId = null}
        //    };
        //    _unitOfWork.Setup(u => u.Repository).Returns(mockRepository.Object);
        //    mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
        //    mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()));
        //    mockRepository.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
        //    //Act
        //    var ex = await Assert.ThrowsAsync<ArgumentNullException>( async() => await _categoryController.CreateCategoryAsync(cat1));
        //    //Assert
        //    mockRepository.Verify(r => r.AddAsync<Category>(cat1), Times.Never);
        //    mockRepository.Verify(r => r.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Never);
        //    mockRepository.Verify(mockRepository => mockRepository.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
        //    Assert.Contains("reference is null", ex.Message);
        //}

        //[Fact(DisplayName = "CreateCategoryAsync method throws ArgumentException")]
        //public async Task CreateCategoryAsyncTest_Throws_ArgumentException()
        //{
        //    //Arange
        //    Category cat1 = new Category { Id = 3, Name = "First set", ParentId = null };
        //    Category cat2 = new Category { Id = 4, Name = "", ParentId = 100000 };
        //    var mockRepository = new Mock<IRepository>();
        //    List<Category> mockDB = new List<Category> {
        //        new Category { Id = 1 , Name = "First set", ParentId = null},
        //        new Category { Id = 2 , Name = "Second set" , ParentId = null}
        //    };
        //    _unitOfWork.Setup(u => u.Repository).Returns(mockRepository.Object);
        //    mockRepository.Setup(r => r.AddAsync<Category>(It.IsAny<Category>())).Callback((Category some) => mockDB.Add(some));
        //    mockRepository.Setup(r => r.GetByIdAsync<Category>(It.IsAny<int>())).ReturnsAsync((int a) => mockDB.FirstOrDefault(x => x.Id == a));
        //    mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(x => string.Equals(x.Name, cat1.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat1.ParentId))
        //        .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat1.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat1.ParentId));
        //    mockRepository.Setup(r => r.SingleOrDefaultAsync<Category>(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId))
        //        .ReturnsAsync(() => mockDB.SingleOrDefault(x => string.Equals(x.Name, cat2.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == cat2.ParentId));
        //    //Act + Assert
        //    var aEx = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.CreateCategoryAsync(cat1));
        //    var bEx = await Assert.ThrowsAsync<ArgumentException>(async () => await _categoryController.CreateCategoryAsync(cat2));
        //    mockRepository.Verify(mockRepository => mockRepository.GetByIdAsync<Category>(It.IsAny<int>()), Times.Never);
        //    mockRepository.Verify(r => r.SingleOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()), Times.Exactly(1));
        //    Assert.Contains("exists", aEx.Message);
        //    Assert.Contains("empty", bEx.Message);
        //}


        public class MockCategoryController : CategoryController
        {
            public MockCategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger) : base(unitOfWork, logger)
            {
            }

            public new ILogger Logger { get { return Logger; } set { Logger = value; } }
            public new IUnitOfWork UnitOfWork { get { return UnitOfWork; } set { UnitOfWork = value; } }
        }

    }
}
