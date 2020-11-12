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
    public class IngredientDetailControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private IngredientDetailController _ingredientDetailController;
        private Mock<ILogger<IngredientDetailController>> _loggerMock;
        private Mock<IRepository> _mockRepository;

        public IngredientDetailControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRepository>();
            _loggerMock = new Mock<ILogger<IngredientDetailController>>();
            _unitOfWork.SetupGet(u => u.Repository).Returns(_mockRepository.Object);
            _ingredientDetailController = new IngredientDetailController(_unitOfWork.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task DeleteIngredientDetailByIdsAsync_Should_Delete_IngredientDetail()
        {
            //Arrange
            List<IngredientDetail> mockIngredientDetailDB = GetDetails();
            IngredientDetail toDelete = new IngredientDetail { RecipeId = 1, IngredientId = 2 };
            _mockRepository.Setup(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()))
                .ReturnsAsync(() => mockIngredientDetailDB.FirstOrDefault(x => x.RecipeId == toDelete.RecipeId && x.IngredientId == toDelete.IngredientId));
            _mockRepository.Setup(r => r.DeleteAsync<IngredientDetail>(It.Is<IngredientDetail>(entity => entity.RecipeId == toDelete.RecipeId && entity.IngredientId == toDelete.IngredientId)))
                .Callback((IngredientDetail ingredientDetail) =>
                {
                    mockIngredientDetailDB.Remove(ingredientDetail);
                });
            //Act
            await _ingredientDetailController.DeleteIngredientDetailByIdsAsync( toDelete.RecipeId, toDelete.IngredientId);
            //Assert
            var mustBeNull = mockIngredientDetailDB.FirstOrDefault(x => x.RecipeId == toDelete.RecipeId && x.IngredientId == toDelete.IngredientId);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync<IngredientDetail>(It.IsAny<Expression<Func<IngredientDetail, bool>>>()), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync<IngredientDetail>(It.Is<IngredientDetail>(entity => entity.RecipeId == toDelete.RecipeId && entity.IngredientId == toDelete.IngredientId)), Times.Once);
            Assert.Null(mustBeNull);
        }
        private List<IngredientDetail> GetDetails()
        {
            return new List<IngredientDetail>
            {
                new IngredientDetail{ RecipeId = 1, IngredientId = 1, Amount =50, MeasureId = 1},
                new IngredientDetail{ RecipeId = 1, IngredientId = 2, Amount =30, MeasureId = 2},
                new IngredientDetail{ RecipeId = 1, IngredientId = 3, Amount =2, MeasureId = 3},
            };
        }
    }
}
