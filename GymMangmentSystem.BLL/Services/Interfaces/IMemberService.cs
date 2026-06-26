using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
    
        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default);

        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct);

        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct);
        Task<bool> UpdateMemberDetailsAsync(int memberId , MemberToUpdateViewModel model, CancellationToken ct);
        Task<bool> RemoveMemberAsync (int memberId , CancellationToken ct);
    }
}
