using AutoMapper;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.Enums;
using ParkingManagementSystem.DAL.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Services
{
    public class LogService : ILogger
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LogService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteLog(LogRequest log)
        {
            var repository = _unitOfWork.GetRepository<Log>();

            var logging = await repository.GetAsync(log.Id);

            if (logging == null)
                throw new ArgumentException(nameof(logging));

            logging.Id = log.Id;
            logging.UpdatedAt = DateTime.Now;
            logging.DeletedAt = DateTime.Now;
            logging.IsDeleted = true;
            logging.IsActive = false;

            await repository.UpdateAsync(logging);

            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> InsertLogAsync(LogLevelType logLevel, string shortMessage, string fullMessage = "")
        {
            var repository = _unitOfWork.GetRepository<Log>();

            var logRequest = new LogRequest
            {
                LogLevelId = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage
            };
            var entity = _mapper.Map<Log>(logRequest);
            entity.ShortMessage = shortMessage;
            entity.FullMessage = fullMessage;
            entity.LogLevelId = logLevel;
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            entity.IsActive = true;

            await repository.AddAsync(entity);

            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }
    }
}
