using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; } 

        #region Relationships

        // One to One with HealthRecord
        public HealthRecord HealthRecord { get; set; } = default!;

        //many to many with Plan
        public ICollection<MemberShip> MemberShips { get; set; } = default!;

        // One to Many with Booking
        public ICollection<Booking> Bookings { get; set; } = default!;

        #endregion

    }

}