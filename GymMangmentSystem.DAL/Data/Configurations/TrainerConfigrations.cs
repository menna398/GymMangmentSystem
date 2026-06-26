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
    internal class TrainerConfigrations : GymUserConfigrations<Trainer>, IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {

            builder.Property(T => T.CreatedAt)
                .HasColumnName("HireDate")
                .HasDefaultValueSql("GETDATE()");


            base.Configure(builder);
        }
    }
}
