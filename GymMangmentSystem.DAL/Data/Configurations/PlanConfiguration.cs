using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymMangmentSystem.DAL.Data.Models;

namespace GymMangmentSystem.DAL.Data.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
               .HasDefaultValueSql("GETDATE()");

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("PlanDurationChecks", "DurationDays Between 1 AND 365");
            });
        }
    }
}
