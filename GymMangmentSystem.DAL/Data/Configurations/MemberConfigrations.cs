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
    internal class MemberConfigrations : GymUserConfigrations<Member> , IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(M => M.CreatedAt)
                .HasColumnName("JoinedDate")
                .HasDefaultValueSql("getdate()");


            base.Configure(builder);
        }
    }
}
