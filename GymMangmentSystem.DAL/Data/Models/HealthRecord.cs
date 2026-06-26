using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? Note { get; set; } 
        public string BloodType { get; set; } = default!;

        #region Relationships

        // One to One with Member
        public Member Member { get; set; }
        public int MemberId { get; set; }

        #endregion

    }
}
