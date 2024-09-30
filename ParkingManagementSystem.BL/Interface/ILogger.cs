using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Interface
{
    public interface ILogger : IBusinessUnit
    {
        Task<bool> InsertLogAsync(LogLevelType logLevel, string shortMessage, string fullMessage = "");
        Task<bool> DeleteLog(LogRequest log);
    }
}
