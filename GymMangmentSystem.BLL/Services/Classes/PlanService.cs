using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var planViewModel = plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive,
                Price = p.Price,
                Description = p.Description,
                DurationDays = p.DurationDays
            });
            return planViewModel;
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId , ct);
            if(plan is null)
            {
                return null;
            }

           return new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
               IsActive = plan.IsActive,
                Price = plan.Price,
                Description = plan.Description,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<UpdatePlanViewModel> GetPlanToUpdateAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync (planId , ct);
            if (plan is null || !plan.IsActive) return null;
            if( await HasActiveMembershipsAsync(planId,ct)) return null;
            return new UpdatePlanViewModel()
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return false;

            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
            
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return false;
            if(await HasActiveMembershipsAsync(id,ct)) return false;

            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        private async Task<bool> HasActiveMembershipsAsync(int planId , CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync(ct:ct);

            return members.Any(m =>
                m.PlanId == planId &&
                m.EndDate>DateTime.Now);
        }
    }
}
