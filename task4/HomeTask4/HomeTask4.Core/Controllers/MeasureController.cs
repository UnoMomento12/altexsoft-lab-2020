using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTask4.Core.Controllers
{
    public class MeasureController : BaseController
    {
        public MeasureController(IUnitOfWork unitOfWork, ILogger<MeasureController> logger) : base(unitOfWork, logger)
        {
        }
        public async Task CreateMeasureAsync(Measure toCreate)
        {
            if (toCreate == null) throw new ArgumentException("Measure reference is null.");
            if (toCreate.Name == null) throw new ArgumentException("Measure name is null!");
            var retrieved = await UnitOfWork.Repository.FirstOrDefaultAsync<Ingredient>(x => x.Name.ToLower() == toCreate.Name.ToLower());
            if (retrieved != null)
            {
                throw new ArgumentException("This Measure already exists");
            }
            await UnitOfWork.Repository.AddAsync<Measure>(toCreate);
        }

        public Task<List<Measure>> GetAllMeasuresAsync()
        {
            return UnitOfWork.Repository.ListAsync<Measure>();
        }
        public async Task DeleteMeasureByIdAsync(int id)
        {
            Measure toDelete = await UnitOfWork.Repository.GetByIdAsync<Measure>(id);
            await UnitOfWork.Repository.DeleteAsync<Measure>(toDelete);
        }
        public Task<Measure> GetMeasureByIdAsync(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Measure>(id);
        }
        public async Task UpdateMeasureAsync(Measure toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Measure>(toUpdate.Id);
            retrieved.Name = toUpdate.Name;
            await UnitOfWork.Repository.UpdateAsync(retrieved);
        }
    }
}
