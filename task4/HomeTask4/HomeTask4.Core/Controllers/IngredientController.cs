using Microsoft.Extensions.Logging;
using HomeTask4.SharedKernel.Interfaces;
using System.Threading.Tasks;
using System;
using HomeTask4.Core.Entities;
using Castle.Core.Internal;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HomeTask4.Core.Controllers
{
    public class IngredientController : BaseController 
    {
        public IngredientController(IUnitOfWork unitOfWork, ILogger<IngredientController> logger) : base(unitOfWork, logger)
        {
        }
        public async Task CreateIngredientAsync(Ingredient toCreate)
        {
            if (toCreate == null) throw new ArgumentException("Ingredient reference is null.");
            if (toCreate.Name.IsNullOrEmpty()) throw new ArgumentException("Ingredient name is null or empty!");
            var retrieved = await UnitOfWork.Repository.FirstOrDefaultAsync<Ingredient>(x => x.Name.ToLower() == toCreate.Name.ToLower());
            if (retrieved != null)
            {
                throw new ArgumentException("This ingredient already exists");
            }
            await UnitOfWork.Repository.AddAsync<Ingredient>(toCreate);
        }

        public Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return UnitOfWork.Repository.ListAsync<Ingredient>();
        }
        public async Task DeleteIngredientByIdAsync(int id)
        {
            Ingredient toDelete = await UnitOfWork.Repository.GetByIdAsync<Ingredient>(id);
            await UnitOfWork.Repository.DeleteAsync<Ingredient>(toDelete);
        }
        public Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Ingredient>(id);
        }
        public async Task UpdateIngredientAsync(Ingredient toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Ingredient>(toUpdate.Id);
            retrieved.Name = toUpdate.Name;
            await UnitOfWork.Repository.UpdateAsync(retrieved);
        }
    }
}
