using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Booking : BaseEntity
    {
        public bool IsAttended { get; set; }

        #region Relationships

        // Many to One with Member
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        // Many to One with Session
        public Session Session { get; set; } = default!;
        public int SessionId { get; set; }

        #endregion
    }
}
