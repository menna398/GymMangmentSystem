using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct);
        Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int planId, CancellationToken ct);
        Task<bool> ToggleActivationAsync(int planId, CancellationToken ct);
        Task<bool> UpdatePlanAsync(int id ,  UpdatePlanViewModel model , CancellationToken ct = default);

    }
}
