using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Entity
{
    [Index(nameof(IsDeleted))]

    public class BaseEntity 
    {
        public BaseEntity()
        {
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public bool IsActive { get; set; }


        public DateTime CreatedAt { get; set; }

        public long? CreatedById { get; set; }


        public DateTime UpdatedAt { get; set; }

        public long? UpdatedById { get; set; }


        public DateTime? DeletedAt { get; set; }

        public long? DeletedById { get; set; }

        public bool IsDeleted { get; set; }
    }
}
