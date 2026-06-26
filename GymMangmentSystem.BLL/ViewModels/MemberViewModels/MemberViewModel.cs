using GymMangmentSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.ViewModels.MemberViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Photo { get; set; } = default!;
        public Gender Gender { get; set; }

        public string Address { get; set; } 
        public string PlanName { get; set; } 
        public string MembershipStartDate { get; set; }
        public string MembershipEndDate { get; set; }
        public String DateOfBirth { get; set; }

    }
}
