using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using AutoMapper;

namespace Application.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Result, ResultDto>();
            CreateMap<ResultDto, Result>();
            CreateMap<BrokerUpdateDto, Result>();
            CreateMap<ResultCreationDto, Result>();
            CreateMap<Result, InitializedResultDto>();
            CreateMap<PayCreationDto, Result>();
            CreateMap<Result, PaidForResultDto>();
        }
    }
}