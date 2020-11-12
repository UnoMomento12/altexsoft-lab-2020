using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
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
            string errorM = "";
            if (toCreate == null)
            {
                errorM = "Measure reference is null.";
                Logger.LogError(errorM);
                throw new ArgumentException(errorM);
            }
            if (toCreate.Name == null)
            {
                errorM = "Measure name is null!";
                Logger.LogError(errorM);
                throw new ArgumentException(errorM);
            }
            var retrieved = await UnitOfWork.Repository.FirstOrDefaultAsync<Measure>(x => x.Name.ToLower() == toCreate.Name.ToLower());
            if (retrieved != null)
            {
                errorM = "This Measure already exists";
                Logger.LogError(errorM);
                throw new ArgumentException(errorM);
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
            if (toDelete != null)
            {
                await UnitOfWork.Repository.DeleteAsync<Measure>(toDelete);
            }
        }
        public Task<Measure> GetMeasureByIdAsync(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Measure>(id);
        }
        public async Task UpdateMeasureAsync(Measure toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Measure>(toUpdate.Id);
            if (retrieved != null)
            {
                retrieved.Name = toUpdate.Name;
                await UnitOfWork.Repository.UpdateAsync(retrieved);
            }
        }
    }
}
