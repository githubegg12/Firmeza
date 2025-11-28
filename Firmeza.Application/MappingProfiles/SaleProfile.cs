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
            .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Will be populated manually or via includes
            .ForMember(dest => dest.UserEmail, opt => opt.Ignore()) // Will be populated manually or via includes
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SaleDetails));
        
        // SaleDetail -> SaleDetailDto
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Total));
        
        // CreateSaleDto -> Sale (basic mapping, details handled in controller)
        CreateMap<CreateSaleDto, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDetails, opt => opt.Ignore())
            .ForMember(dest => dest.ReceiptUrl, opt => opt.Ignore());
        
        // UpdateSaleDto -> Sale (partial update)
        CreateMap<UpdateSaleDto, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDate, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDetails, opt => opt.Ignore())
            .ForMember(dest => dest.ReceiptUrl, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
