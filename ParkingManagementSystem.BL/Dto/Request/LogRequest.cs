using Castle.MicroKernel.SubSystems.Conversion;
using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Request
{
    public class LogRequest : BaseRequest
    {
        public LogLevelType LogLevelId { get; set; }

        public string ShortMessage { get; set; }

        public string FullMessage { get; set; }
    }
}
