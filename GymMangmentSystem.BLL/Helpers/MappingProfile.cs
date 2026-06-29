using AutoMapper;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();
        }

        private void MapMember()
        {
            CreateMap<CreateMemberViewModel, Member>()
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
               {
                   BuildingNumber = src.BuildingNumber,
                   Street = src.Street,
                   City = src.City
               }))
               .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

            CreateMap<HealthRecordViewModel, HealthRecord>()
               .ReverseMap(); // a7wel ben el etnen 3ady " 2 way mapping role"

            CreateMap<Member, MemberViewModel>()
                    //.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));

        }

        private void MapSession()
        {
            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Session, SessionViewModel>();

            CreateMap<Trainer, TrainerSelectViewModel>();

            CreateMap<Category, CategorySelectViewModel>();

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }


    }
}
