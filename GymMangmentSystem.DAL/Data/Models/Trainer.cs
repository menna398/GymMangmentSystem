using GymMangmentSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Specialties Specialties { get; set; }

        #region Relationships

        // One to Many with Sessions
        public ICollection<Session> Sessions { get; set; } = default!;


        #endregion

    }
}
