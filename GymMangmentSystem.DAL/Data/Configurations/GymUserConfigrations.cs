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
    internal class GymUserConfigrations<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(U => U.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(U => U.Email)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder.Property(U => U.Phone)
                .HasColumnType("varchar")
                .HasMaxLength(11);

            builder.HasIndex(builder => builder.Email)
                .IsUnique();
            builder.HasIndex(builder => builder.Phone)
                .IsUnique();

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("Phonecheck", "Phone LIKE '01[0125][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'"); // 011 . 012 , 015 , 010 & then 8 digits
                TB.HasCheckConstraint("EmailCheck", "Email LIKE '_%@_%._%'"); // username@gmail.com
            });

            builder.OwnsOne(U=> U.Address, Address =>
            {
                Address.Property(A => A.City)
                    .HasColumnType("varchar")
                    .HasMaxLength(30);
                Address.Property(A => A.Street)
                    .HasColumnType("varchar")
                    .HasMaxLength(30);
            });

        }
    }
}
