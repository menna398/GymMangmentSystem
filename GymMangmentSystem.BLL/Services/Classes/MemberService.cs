using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        public IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<MemberShip> _membershipRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public MemberService(IGenericRepository<Member> memberRepo,
            IGenericRepository<MemberShip> membershipRepo,
            IGenericRepository<Plan> planRepo,
            IGenericRepository<HealthRecord> healthRecordRepo,
            IGenericRepository<Booking> bookingRepo)
        {
            _memberRepo = memberRepo;
            _membershipRepo = membershipRepo;
            _planRepo = planRepo;
            _healthRecordRepo = healthRecordRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //check mail
            var emailExists = await _memberRepo.AnyAsync(m => m.Email == model.Email, ct);

            //check phone
            var phoneExists = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExists || phoneExists)
            {
                return false;
            }

            //mapping view model , add to db
            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                HealthRecord = new HealthRecord
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note
                }
            };
            var result = await _memberRepo.AddAsync(member, ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepo.GetAllAsync(ct: ct);

            if (!members.Any())
                return [];

            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Photo = m.Photo,
                Gender = m.Gender
            });

            return memberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(memberId);

            if (member == null)
            {
                return null;
            }

            var memberViewModel = new MemberViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Gender = member.Gender,
                Phone = member.Phone,
                Photo = member.Photo,
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
            };

            var memberShip = await _membershipRepo.FirstOrDefaultAsync(M => M.MemberId == memberId && M.EndDate > DateTime.Now, ct: ct);
            if (memberShip is not null)
            {
                memberViewModel.MembershipStartDate = memberShip.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = memberShip.EndDate.ToShortDateString();
                var plan = await _planRepo.GetByIdAsync(memberShip.PlanId, ct);
                memberViewModel.PlanName = plan?.Name!;
            }

            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct)
        {
            var healthRecord = await _healthRecordRepo.FirstOrDefaultAsync(H => H.MemberId == memberId, ct: ct);
            if (healthRecord is null)
            {
                return null;
            }

            return new HealthRecordViewModel()
            {
                Height = healthRecord.Height,
                Weight = healthRecord.Weight,
                BloodType = healthRecord.BloodType,
                Note = healthRecord.Note
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if (member is null)
            {
                return null;
            }

            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City
            };

        }

        public async Task<bool> UpdateMemberDetailsAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if (member is null)
            {
                return false;
            }

            var emailExists = await _memberRepo.AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct); //34an ata2k en el mail mykon4 m3a 7ad 8ery
            var phoneExists = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExists || phoneExists)
            {
                return false;
            }

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;

            member.UpdatedAt = DateTime.Now;

            var result = await _memberRepo.UpdateAsync(member, ct);
            return result > 0;
        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if(member is null)
            {
                return false;
            }
            
            var futureSessions = await _bookingRepo.AnyAsync(b=>b.MemberId == memberId && b.Session.StartDate>DateTime.Now , ct);
            if (futureSessions)
            {
                return false;
            }

            var result = await _memberRepo.DeleteAsync(member, ct);
            return result > 0;
        }
    }
}
