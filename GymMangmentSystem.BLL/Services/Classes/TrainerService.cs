using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.BLL.ViewModels.TrainerViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            var trainersViewModel = trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Gender = t.Gender.ToString(),
                Address = $"{t.Address.BuildingNumber} - {t.Address.Street} - {t.Address.City}",
                DateOfBirth = t.DateOfBirth.ToShortDateString(),
                Specialties = t.Specialties.ToString()
            });
            return trainersViewModel;
        }

        public async Task<TrainerViewModel> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return null; 
            return new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}",
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Specialties = trainer.Specialties.ToString()
            };
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct))
                return false;

            // Check for unique Phone
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct))
                return false;

            // Map ViewModel to Entity
            var entity = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
                DateOfBirth = model.DateOfBirth,
                Specialties = model.Specialties
            };

            _unitOfWork.GetRepository<Trainer>().AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }


        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdate(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return null;

            return new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialties = trainer.Specialties,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City
            };
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null)
                return false;

            var hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(
                s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);

            if (hasFutureSessions)
                return false;

            _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null)
                return false;

            // Check for unique Email (excluding current trainer)
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct))
                return false;

            // Check for unique Phone (excluding current trainer)
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct))
                return false;

            // Update fields
            trainer.Name = model.Name; 
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Specialties = model.Specialties;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
