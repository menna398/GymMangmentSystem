namespace GymMangmentSystem.DAL.Data.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        #region RElationships

        // many to many with Member
        public ICollection<MemberShip> MemberShips { get; set; } = default!;

        #endregion
    }
}
