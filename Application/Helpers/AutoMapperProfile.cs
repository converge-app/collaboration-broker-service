using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using AutoMapper;

namespace Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Broker, BrokerDto>();
            CreateMap<BrokerDto, Broker>();
            CreateMap<BrokerUpdateDto, Broker>();
            CreateMap<BrokerCreationDto, Broker>();
        }
    }
}