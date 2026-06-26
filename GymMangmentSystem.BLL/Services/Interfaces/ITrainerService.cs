using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using GymMangmentSystem.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId , CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdate (int trainerId , CancellationToken ct =default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<bool> UpdateTrainerDetailsAsync( int trainerId , TrainerToUpdateViewModel model , CancellationToken ct = default);
        Task<bool> RemoveTrainerAsync(int trainerId , CancellationToken ct = default );

    }
}
