using GymMangmentSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Configurations
{
    public class HealthRecordConfigrations : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(H => H.BloodType)
                .HasMaxLength(3);

            builder.Property(H=>H.Note)
                .HasMaxLength(500);
        }
    }
}
