using AlumniWebsite.API.Model;
using AlumniWebsite.API.ModelDto;
using AlumniWebsite.API.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.MapperConfiguration
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<Member, MemberListDto>()
                .ForMember(destination => destination.PhotoUrl, option =>
                {
                    option.MapFrom(source => source.Photos.FirstOrDefault(active => active.IsMain).Url);
                })
                .ForMember(destination => destination.Age, option =>
                  {
                      option.MapFrom(source => source.DateOfBirth.CalculateAge());

                  });
            CreateMap<Member, RegisterDto>().ReverseMap();
            CreateMap<MemberListDto, Member>();
            CreateMap<Member, MemberDetailsDto>()
                .ForMember(destin => destin.PhotoUrl, option =>
                {
                    option.MapFrom(p => p.Photos.FirstOrDefault(i => i.IsMain).Url);
                })
                .ForMember(destination => destination.Age, option =>
                {
                    option.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UpdateDto, Member>();
            CreateMap<MessageForCreateDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturn>()
                .ForMember(s => s.SenderPhotoUrl,
                option => option.MapFrom(m => m.Sender.Photos.FirstOrDefault(i => i.IsMain).Url))
                .ForMember(s => s.RecipientPhotoUrl, option => option.MapFrom(m => m.Recipient.Photos.FirstOrDefault(i => i.IsMain).Url));



        }
    }
}
