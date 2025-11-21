using AutoMapper;
using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.MappingProfiles;

/// <summary>
/// AutoMapper profile for Product entity mappings
/// </summary>
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Product -> ProductDto
        CreateMap<Product, ProductDto>();
        
        // CreateProductDto -> Product
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SaleDetails, opt => opt.Ignore());
        
        // UpdateProductDto -> Product
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.SaleDetails, opt => opt.Ignore());
    }
}
