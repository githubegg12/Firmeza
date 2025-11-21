using AutoMapper;
using Firmeza.Application.DTOs.Sale;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for Sale entity mappings
/// </summary>
public class SaleProfile : Profile
{
    public SaleProfile()
    {
        // Sale -> SaleDto
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));
        
        // SaleDetail -> SaleDetailDto
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        
        // CreateSaleDto -> Sale (if needed in the future)
        CreateMap<CreateSaleDto, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDetails, opt => opt.Ignore());
    }
}

