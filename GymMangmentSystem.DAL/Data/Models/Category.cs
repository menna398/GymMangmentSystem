using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = default!;

        #region Relationships

        // One to Many with Sessions
        public ICollection<Session> Sessions { get; set; } = default!;

        #endregion
    }
}
