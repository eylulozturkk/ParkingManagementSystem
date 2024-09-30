using Castle.MicroKernel.SubSystems.Conversion;
using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Entity
{
    public class Log : BaseEntity
    {
        public LogLevelType LogLevelId { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string ShortMessage { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string FullMessage  { get; set; }

    }
}
