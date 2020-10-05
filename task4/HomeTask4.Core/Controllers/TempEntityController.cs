using System;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class TempEntityController
    {
        private readonly IUnitOfWork _unitOfWork;

        public TempEntityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TempEntity> AddNewTempEntity()
        {
            var lastId = (await _unitOfWork.Repository.ListAsync<TempEntity>()).Max(x => x.Id);

            var nextId = lastId + 1;

            return await _unitOfWork.Repository.AddAsync<TempEntity>(new TempEntity
            {
                Id = nextId
            });
        }
    }
}
