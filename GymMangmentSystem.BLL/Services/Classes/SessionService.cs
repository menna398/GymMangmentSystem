using AutoMapper;
using GymMangmentSystem.BLL.Helpers;
using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Data.Models.Enums;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork ;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork
            , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);

            if (!sessions.Any()) return [];

            var sessionsViewModel = sessions.OrderByDescending(S => S.StartDate)
                .Select(S => new SessionViewModel()
                {
                    Id = S.Id,
                    Description = S.Description,
                    CategoryName = S.Category.CategoryName,
                    TrainerName = S.Trainer.Name,
                    StartDate = S.StartDate,
                    EndDate = S.EndDate,
                    Capacity = S.Capacity
                });

            foreach (var session in sessionsViewModel)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBokkedSlotsAsync(session.Id, ct);
            }

            return sessionsViewModel;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be after Satrt date");
            if(model.StartDate <= DateTime.Now) return Result.Validation("Start date must be in the Future");

            var trainerRepo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer not Found");

            if (await _unitOfWork.SessionRepository.AnyAsync(
                S => S.TrainerId == model.TrainerId &&
                (
                    (S.StartDate <= model.StartDate && S.EndDate >= model.StartDate) ||
                    (model.StartDate <= S.StartDate && model.EndDate >= S.StartDate)
                ),
                ct))
            {
                return Result.Fail("The Trainer has another Session in same Time Range , He's Busyy!");
            } //for sessions overlapping

            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category not Found");

            var IsValidSpeciality = Enum.TryParse<Specialties>(category.CategoryName, true , out var categorySpeciality );

            if (!IsValidSpeciality || trainer.Specialties != categorySpeciality) return Result.Validation("Cannot Create This Session for this Trainer");

            var session = _mapper.Map<Session>(model);

            _unitOfWork.SessionRepository.AddAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Session") ;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);
            if(session is null) return null;

            var sessionViewModel = _mapper.Map<SessionViewModel>(session);
            sessionViewModel.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBokkedSlotsAsync(sessionId, ct);

            return sessionViewModel;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return null;

            if (!await IsValidSessionForUpdateAsunc(session, ct)) return null;

            return _mapper.Map<UpdateSessionViewModel>(session);

        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not Found");

            if (session.StartDate <= DateTime.Now) return Result.Fail("Cannot Edit Session that has Already Started");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBokkedSlotsAsync(session.Id, ct);
            if (bookingCount > 0) return Result.Fail("Cannot Edit a Session that has Bookings");

            if (model.EndDate <= model.StartDate) return Result.Validation("End Date must be After Start Date");

            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date must be in the Future");

            var trainerRepo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound("TRainer Not Found");

            var isUnAvaliable = await _unitOfWork.SessionRepository
                .AnyAsync(S => S.TrainerId == model.TrainerId && S.Id != sessionId &&
                ((S.StartDate <= model.StartDate && S.EndDate >= model.StartDate) || (model.StartDate <= S.StartDate && model.EndDate >= S.StartDate)), ct);

            if (isUnAvaliable) return Result.Fail("The Trainer has another Session in same Time Range , He,s Busyy!");

            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetByIdAsync(model.CategoryId, ct);

            var isValidSpeciality = Enum.TryParse<Specialties>(category.CategoryName.Trim(), true, out var categorySpeciality);

            if(!isValidSpeciality || categorySpeciality != trainer.Specialties ) return Result.Validation("Cannot Create This Session for this Trainer");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.UpdateAsync(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Session");

        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not Found");

            if (session.StartDate <= DateTime.Now) return Result.Fail("Cannot Delete Ongoing Or Completed Session");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBokkedSlotsAsync(session.Id, ct);
            if (bookingCount > 0) return Result.Fail("Cannot Delete a Session that has Bookings");

            _unitOfWork.SessionRepository.DeleteAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Session");
        }


        private async Task<bool> IsValidSessionForUpdateAsunc (Session session , CancellationToken ct)
        {
            if(session.StartDate < DateTime.Now) return false;

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBokkedSlotsAsync(session.Id, ct);
            return bookingCount == 0;

        }

    }
}
