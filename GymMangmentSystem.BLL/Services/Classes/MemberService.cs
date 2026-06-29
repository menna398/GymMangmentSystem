using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork 
          //  , IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            //_mapper = mapper;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //check mail
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);

            //check phone
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExists || phoneExists)
            {
                return false;
            }

            //mapping view model , add to db
            var member = //_mapper.Map<CreateMemberViewModel, Member>(model);
                new Member
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

            _unitOfWork.GetRepository<Member>().AddAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

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
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);

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

            var memberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(M => M.MemberId == memberId && M.EndDate > DateTime.Now, ct: ct);
            if (memberShip is not null)
            {
                memberViewModel.MembershipStartDate = memberShip.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = memberShip.EndDate.ToShortDateString();
                var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(memberShip.PlanId, ct);
                memberViewModel.PlanName = plan?.Name!;
            }

            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(H => H.MemberId == memberId, ct: ct);
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
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
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
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null)
            {
                return false;
            }

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct); //34an ata2k en el mail mykon4 m3a 7ad 8ery
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);

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

            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if(member is null)
            {
                return false;
            }
            
            var futureSessions = await _unitOfWork.GetRepository<Booking>().AnyAsync(b=>b.MemberId == memberId && b.Session.StartDate>DateTime.Now , ct);
            if (futureSessions)
            {
                return false;
            }

            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var result =  await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
