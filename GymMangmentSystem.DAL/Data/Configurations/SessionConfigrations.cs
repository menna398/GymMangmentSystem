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
    internal class SessionConfigrations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(TB =>{
                TB.HasCheckConstraint("CapacityCheck", "Capacity between 1 and 25");
                TB.HasCheckConstraint("DateCheck", "StartDate < EndDate");
            });


        }
    }
}
