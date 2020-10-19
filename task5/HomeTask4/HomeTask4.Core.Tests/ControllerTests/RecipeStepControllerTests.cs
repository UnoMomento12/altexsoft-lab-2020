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
    public class RecipeStepControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private RecipeStepController _recStepCont;

        public RecipeStepControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _recStepCont = new RecipeStepController(_unitOfWork.Object, new LoggerFactory().CreateLogger<RecipeStepController>());
        }
        [Fact(DisplayName = "AddStepsAsync method adds RecipeStep to DB ")]
        public async Task AddStepsAsync()
        {
            //Arrange
            List<RecipeStep> recipeStepsDB = new List<RecipeStep>();
            var repos = new Mock<IRepository>();
            _unitOfWork.Setup(u => u.Repository).Returns(repos.Object);
            repos.Setup(r => r.AddRangeAsync<RecipeStep>(It.IsAny<List<RecipeStep>>()))
                 .Callback((List<RecipeStep> some) =>
                 {
                     recipeStepsDB.AddRange(some);
                 });
            List<RecipeStep> stepsToAdd = new List<RecipeStep>
            {
                new RecipeStep{ Id = 1 , Description = "step 1.1" , RecipeId = 1, StepNumber = 1},
                new RecipeStep{ Id = 2 , Description = "step 1.2" , RecipeId = 1, StepNumber = 2},
                new RecipeStep{ Id = 3 , Description = "step 1.3" , RecipeId = 1, StepNumber = 3},
                new RecipeStep{ Id = 4 , Description = "step 2.1" , RecipeId = 2, StepNumber = 1},
                new RecipeStep{ Id = 5 , Description = "step 2.2" , RecipeId = 2, StepNumber = 2},
                new RecipeStep{ Id = 6 , Description = "step 2.3" , RecipeId = 2, StepNumber = 3},
            };
            //Act
            await _recStepCont.AddStepsAsync(stepsToAdd);
            //Assert
            repos.Verify(repos => repos.AddRangeAsync<RecipeStep>(It.IsAny<List<RecipeStep>>()), Times.Once);
            Assert.Equal(recipeStepsDB, stepsToAdd);
        }

    }
}
