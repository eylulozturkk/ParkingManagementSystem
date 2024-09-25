using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Context
{
    public interface IDataContext
    {
        DbSet<Provider> Provider { get; set; }
        DbSet<Template> Template { get; set; }
        DbSet<Setting> Settings { get; set; }
    }
}
