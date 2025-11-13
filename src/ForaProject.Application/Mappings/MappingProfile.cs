using AutoMapper;
using ForaProject.Application.DTOs;
using ForaProject.Domain.Aggregates;
using ForaProject.Domain.Entities;

namespace ForaProject.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Company mappings
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.Cik, opt => opt.MapFrom(src => src.Cik.Value))
            .ForMember(dest => dest.StandardFundableAmount, opt => opt.MapFrom(src => src.StandardFundableAmount))
            .ForMember(dest => dest.SpecialFundableAmount, opt => opt.MapFrom(src => src.SpecialFundableAmount));

        // IncomeRecord mappings
        CreateMap<IncomeRecord, IncomeRecordDto>();

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.ToList()));
    }
}
