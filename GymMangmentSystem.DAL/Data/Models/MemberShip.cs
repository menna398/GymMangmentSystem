using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class MemberShip : BaseEntity
    {
        public DateTime EndDate { get; set; }

        public string Status => DateTime.Now > EndDate ? "Expired" : "Active";

        public bool IsActive => DateTime.Now < EndDate;

        #region Relationships

        // Many to One with Member
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        // Many to One with Plan
        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; }

        #endregion

    }
}
