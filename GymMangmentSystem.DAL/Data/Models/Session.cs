using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; }=default!;

        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #region Relationships

        // Many to One with Trainer
        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; }

        // Many to One with Category
        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }

        // One to Many with Booking
        public ICollection<Booking> Bookings { get; set; } = default!;

        #endregion

    }
}
